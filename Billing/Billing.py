import json
import requests
import os
from requests.auth import HTTPBasicAuth 
from datetime import datetime
from datetime import timedelta
import webbrowser

global SANDBOX
SANDBOX = False

global PAYPAL_URL 
PAYPAL_URL = r"https://api-m.sandbox.paypal.com" if SANDBOX else r"https://api-m.paypal.com"

global EMAIL
EMAIL = 'sb-o3f7i5445250@business.example.com' if SANDBOX else 'saadtaimoor@teraflare.dev'

def main():
    print('BEGINNING BILLING PROCESS')

    # Confirm month (if no input, use today's month)
    print('Getting month...')
    month = GetMonth()

    # Get azure consumption data
    print("Getting data...")
    data = GetAzureConsumptionData(month)

    # Get cost 
    print("Getting Cost...")
    finalCost = GetCost(data)
    
    # Confirm before sending
    input("Please verify cost...")

    # Get paypal access token
    print("Generating Paypal API Token...")
    PaypalAccessToken = GetPaypalAccessToken()

    # Query paypal for the next invoice
    print("Geting paypal invoice number...")
    invoiceNumber = GetInvoiceNumber(PaypalAccessToken)

    # Generate paypal invoice
    print("Generating Paypal Invoice")
    sendInvoiceUrl = GenerateInvoice(PaypalAccessToken, invoiceNumber, finalCost)

    # Send paypal invoice
    print("Sending invoice...")
    SendInvoice(PaypalAccessToken, sendInvoiceUrl)

    #Viewing Invoice
    print("Invoice sent!")
    GetInvoiceView(PaypalAccessToken, sendInvoiceUrl)



def GetMonth():
    inputted = input("Enter the month NUMBER (e.g. '3' for March): ")
    try:
        selectedMonth = int(inputted)
        if (selectedMonth > 12 or selectedMonth < 0):
            return GetCurrentMonth()
    finally:
        return GetCurrentMonth()


def GetCurrentMonth():
    return datetime.today().month


def GetAzureConsumptionData(m):
    script = f'az consumption usage list --start-date "2021-{m}-01" --end-date "2021-{m}-31" > Output.json'
    res = os.system(script)
    if (res != 0):
        raise Exception("Failed to get Azure data!")
    
    with open("Output.json", "r") as datafile:
        data = json.load(datafile)
    return data
    

def GetCost(data):
    preTaxCost = 0
    for e in data:
        if e["tags"] and e["tags"]["Project"]:
            preTaxCost += float(e["pretaxCost"])
    
    tax = preTaxCost * 0.13
    postTaxCost = preTaxCost + tax

    print(f"Pretax Cost: ${preTaxCost:.{2}f}")
    print(f"Tax Cost:    ${tax:.{2}f}")
    print(f"Final Cost:  ${postTaxCost:.{2}f}")

    return preTaxCost / 1.05


def GetPaypalAccessToken():
    url = PAYPAL_URL + r"/v1/oauth2/token"

    headers = {
        "Accept": "application/json",
        "Accept-Language": "en_US"
    }

    with open("appSettings.json", "r") as credentialfile:
        credentials = json.load(credentialfile)

    if (SANDBOX):
        auth = HTTPBasicAuth(credentials["client_id_sandbox"], credentials["secret_sandbox"])
    else:
        auth = HTTPBasicAuth(credentials["client_id"], credentials["secret"])

    payload = { "grant_type" : "client_credentials"}

    res = requests.post(url, data=payload, headers=headers, auth=auth)
    if (res.ok == False):
        raise Exception(f"Request failed! Status code {res.status_code}, {res.content}") 

    results = json.loads(res.content)

    accessToken = "Bearer " + results["access_token"]
    token = {"Authorization": accessToken, "Content-Type": "application/json"}
    return token


def GetInvoiceNumber(token):
    url = PAYPAL_URL + r"/v2/invoicing/generate-next-invoice-number"
    res = requests.post(url, headers=token)

    if (res.ok == False):
        raise Exception(f"Request failed! Status code {res.status_code}, {res.content}")

    results = json.loads(res.content)
    invoice = results["invoice_number"]
    print("Invoice number is " + results["invoice_number"])
    return invoice


def GenerateInvoice(token, invoiceNumber, amount):
    url = PAYPAL_URL + r"/v2/invoicing/invoices"
    body = {
        "detail": {
            "invoice_number": invoiceNumber,
            "invoice_date": datetime.now().strftime("%Y-%m-%d"),
            "currency_code": "CAD",
            "note": "Thank you for your business.",
            "term": "",
            "memo": "",
            "payment_term": {
                "due_date": (datetime.now() + timedelta(days=90)).strftime("%Y-%m-%d")
            }
        },
        "invoicer": {
            "name": {
            "given_name": "Saad",
            "surname": "Taimoor"
            },
            "phones": [
            {
                "country_code": "1",
                "national_number": "7808936573",
                "phone_type": "MOBILE"
            }
            ],
            "email_address": EMAIL,
            "website": "https://www.teraflare.dev/",
            "logo_url": "https://i.imgur.com/z8PyMxJ.png"
        },
        "primary_recipients": [
            {
                "billing_info": {
                    "name": {
                        "given_name": "Iffat",
                        "surname": "Taimoor"
                    },
                    "address": {
                        "address_line_1": "1752 32st NW",
                        "admin_area_2": "Edmonton",
                        "admin_area_1": "AB",
                        "postal_code": "T6T 0P5",
                        "country_code": "CA"
                    },
                    "email_address": "iffatt@hotmail.com",
                    "phones": [
                        {
                            "country_code": "001",
                            "national_number": "7804664336",
                            "phone_type": "HOME"
                        }
                    ]
                }
            }
        ],
        "items": [
            {
                "name": "Hosting Charges",
                "description": "Cost of running AlMualim (website, database, and storage charges)",
                "quantity": "1",
                "unit_amount": {
                    "currency_code": "CAD",
                    "value": f"{amount:.{2}f}"
                },
                "tax": {
                    "name": "Sales Tax",
                    "percent": "5"
                }
            }
        ],
        "configuration": {
            "partial_payment": {
                "allow_partial_payment": False
                },
            "allow_tip": True,
            "tax_inclusive": False
        },   
    }

    data = json.dumps(body)

    res = requests.post(url, headers=token, data=data)

    if(res.ok == False):
        raise Exception(f"Request failed! Status code {res.status_code}, {res.content}") 

    results = json.loads(res.content)
    return results["href"]


def SendInvoice(token, url):
    url = url + r"/send"
    res = requests.post(url, headers=token)
    
    if(res.ok == False):
        raise Exception(f"Request failed! Status code {res.status_code}, {res.content}") 

def GetInvoiceView(token, url):
    res = requests.get(url, headers=token)

    if(res.ok == False):
        raise Exception(f"Request failed! Status code {res.status_code}, {res.content}") 

    results = json.loads(res.content)
    if(SANDBOX):
        invoiceUrl = results["detail"]["metadata"]["recipient_view_url"]
    else:
        invoiceUrl = results["detail"]["metadata"]["invoicer_view_url"]

    webbrowser.open_new_tab(invoiceUrl)


if(__name__ == "__main__"):
    main()