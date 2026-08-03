var fs = require("fs");
var path = require("path");

var buildDir = path.resolve(__dirname, "..", "..", "build");

fs.cp(buildDir, "C:\\inetpub\\wwwroot\\CvUpUI", { recursive: true }, (err) => {
  if (err) {
    console.error(err);
  }
});
