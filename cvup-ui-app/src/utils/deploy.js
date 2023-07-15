var fs = require("fs");

fs.cp(
  "C:\\GitHub\\CvUp\\cvup-ui-app\\build",
  "C:\\inetpub\\wwwroot\\CvUpUI",
  { recursive: true },
  (err) => {
    if (err) {
      console.error(err);
    }
  }
);
