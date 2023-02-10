import { observer } from "mobx-react";
import { PdfViewer } from "../components/pdfViewer/PdfViewer";
import "react-quill/dist/quill.snow.css";
import { useEffect } from "react";
import { useParams } from "react-router-dom";
import { useStore } from "../Hooks/useStore";
import styles from "./Cv.module.scss";

export const Cv = observer(() => {
  // let { id } = useParams();
  // const { candsStore } = useStore();

  // useEffect(() => {
  //   (async () => {
  //     await candsStore.getCv(id || "");
  //   })();
  // }, [id]);

  return (
    <div className={styles.scrollCv}>
      fdgdfgdfgdfg
      <br />
      dfgdfgdgdg
      <br />
      dfgdfgdgdg
      <br />
      dfgdfgdgdg
      <br />
      dfgdfgdgdg
      <PdfViewer />
    </div>
  );
});
