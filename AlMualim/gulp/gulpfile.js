/// <binding beforebuild="process:less"></binding>
var gulp = require("gulp"),
    less = require("gulp-less"),
    cleanCss = require("gulp-clean-css"),
    concat = require("gulp-concat"), 
    clean = require("gulp-clean");

var wwwroot = "../wwwroot/css/"
var lessFiles = "../Assets/less/*.less"
 
gulp.task("compile:less", function () {
    return gulp.src(lessFiles)
        .pipe(concat("site.css"))
        .pipe(less())
        .pipe(cleanCss({ compatibility: 'ie8' }))
        .pipe(gulp.dest(wwwroot));
});

gulp.task("watch", function() {
  gulp.watch(lessFiles, gulp.series('compile:less'));
});