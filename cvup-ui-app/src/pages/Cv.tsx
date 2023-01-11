import { observer } from "mobx-react";
import { PdfViewer } from "../components/pdfViewer/PdfViewer";
import "react-quill/dist/quill.snow.css";
import { useEffect } from "react";
import { useParams } from "react-router-dom";
import { useStore } from "../Hooks/useStore";

export const Cv = observer(() => {
  // let { id } = useParams();
  // const { cvsStore } = useStore();

  // useEffect(() => {
  //   (async () => {
  //     await cvsStore.getCv(id || "");
  //   })();
  // }, [id]);

  return <PdfViewer />;
});
