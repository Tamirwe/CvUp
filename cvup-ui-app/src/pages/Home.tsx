import { Grid } from "@mui/material";
import { observer } from "mobx-react";
import { CvsListWrapper } from "../components/cvs/CvsListWrapper";
import { useStore } from "../Hooks/useStore";

export const Home = observer(() => {
  const { cvsStore } = useStore();
  // const [doc, setDoc] = useState(
  //   "http://89.237.94.86:8010/api/Download/GetWord2"
  // );
  // const docs: any = [];

  // useEffect(() => {
  //   loadFile();
  // }, []);

  // const loadFile = async () => {
  //   const fileBase64 = await generalStore.getFileBase64();
  //   const url = URL.createObjectURL(base64toBlob(fileBase64));
  //   setDocs((oldArray: any[]) => [...oldArray, { uri: url }]);
  // };

  // // const docs = [{ uri: "https://localhost:7217/api/Download" }];

  // const base64 =
  //   "data:application/pdf;base64,JVBERi0xLjcKCjEgMCBvYmogICUgZW50cnkgcG9pbnQKPDwKICAvVHlwZSAvQ2F0YWxvZwogIC9QYWdlcyAyIDAgUgo+PgplbmRvYmoKCjIgMCBvYmoKPDwKICAvVHlwZSAvUGFnZXMKICAvTWVkaWFCb3ggWyAwIDAgMjAwIDIwMCBdCiAgL0NvdW50IDEKICAvS2lkcyBbIDMgMCBSIF0KPj4KZW5kb2JqCgozIDAgb2JqCjw8CiAgL1R5cGUgL1BhZ2UKICAvUGFyZW50IDIgMCBSCiAgL1Jlc291cmNlcyA8PAogICAgL0ZvbnQgPDwKICAgICAgL0YxIDQgMCBSIAogICAgPj4KICA+PgogIC9Db250ZW50cyA1IDAgUgo+PgplbmRvYmoKCjQgMCBvYmoKPDwKICAvVHlwZSAvRm9udAogIC9TdWJ0eXBlIC9UeXBlMQogIC9CYXNlRm9udCAvVGltZXMtUm9tYW4KPj4KZW5kb2JqCgo1IDAgb2JqICAlIHBhZ2UgY29udGVudAo8PAogIC9MZW5ndGggNDQKPj4Kc3RyZWFtCkJUCjcwIDUwIFRECi9GMSAxMiBUZgooSGVsbG8sIHdvcmxkISkgVGoKRVQKZW5kc3RyZWFtCmVuZG9iagoKeHJlZgowIDYKMDAwMDAwMDAwMCA2NTUzNSBmIAowMDAwMDAwMDEwIDAwMDAwIG4gCjAwMDAwMDAwNzkgMDAwMDAgbiAKMDAwMDAwMDE3MyAwMDAwMCBuIAowMDAwMDAwMzAxIDAwMDAwIG4gCjAwMDAwMDAzODAgMDAwMDAgbiAKdHJhaWxlcgo8PAogIC9TaXplIDYKICAvUm9vdCAxIDAgUgo+PgpzdGFydHhyZWYKNDkyCiUlRU9G";
  // const pdfContentType = "application/pdf";

  // // const pdfContentType = "application/msword";

  // const base64toBlob = (data: string) => {
  //   // Cut the prefix `data:application/pdf;base64` from the raw base 64
  //   const base64WithoutPrefix = data.substr(
  //     `data:${pdfContentType};base64,`.length
  //   );

  //   const bytes = atob(base64WithoutPrefix);
  //   let length = bytes.length;
  //   let out = new Uint8Array(length);

  //   while (length--) {
  //     out[length] = bytes.charCodeAt(length);
  //   }

  //   return new Blob([out], { type: pdfContentType });
  // };

  // const url = URL.createObjectURL(base64toBlob(base64));

  // const docs = [{ uri: https://localhost:7217/api/Download/GetCv }];

  return (
    <Grid container>
      <Grid item xs={12} md={8}>
        {/* <div style={{ display: "flex" }}> */}
        {/* <button
          onClick={() => {
            setDoc("http://89.237.94.86:8010/api/Download/GetWord");
          }}
        >
          word 1
        </button>
        <button
          onClick={() => {
            setDoc("http://89.237.94.86:8010/api/Download/GetWord2");
          }}
        >
          word 2
        </button>
        <button
          onClick={() => {
            setDoc("http://89.237.94.86:8010/api/Download/GetCv");
          }}
        >
          pdf
        </button> */}
        {/* <div style={{ height: "100vh", width: "60vw", position: "relative" }}> */}
        {/* <img src="http://localhost:8010/api/Download/GetJpg" width={1500} /> */}
        <iframe
          title="iViewDoc"
          seamless
          // src="https://drive.google.com/viewer?embedded=true&hl=en-US&url=http://89.237.94.86:8025/cv_57064.doc"
          src={`https://drive.google.com/viewer?embedded=true&url=${cvsStore.cvId}`}
          style={{
            overflow: "hidden",
            height: "99vh",
            width: "100%",
            border: "0",
            margin: "0",
            padding: "0",
          }}
          width="80%"
        />
        {/* </div> */}
      </Grid>
      <Grid>
        <div>
          <CvsListWrapper />
        </div>
      </Grid>
    </Grid>

    // <div>
    //   <img src="https://localhost:7217/api/Download/GetJpg" width={1500} />
    // </div>
    // <DocViewer
    //   documents={docs}
    //   initialActiveDocument={docs[1]}
    //   pluginRenderers={DocViewerRenderers}
    // />
  );
});
