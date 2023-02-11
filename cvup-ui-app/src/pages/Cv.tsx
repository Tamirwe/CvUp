import { observer } from "mobx-react";
import { PdfViewer } from "../components/pdfViewer/PdfViewer";
import "react-quill/dist/quill.snow.css";
import { useStore } from "../Hooks/useStore";
import styles from "./Cv.module.scss";

export const Cv = observer(() => {
  const { candsStore } = useStore();

  return (
    <div className={styles.scrollCv}>
      <div className="qlCustom">
        <div
          className="ql-editor"
          dangerouslySetInnerHTML={{
            __html: candsStore.candSelected?.review || "",
          }}
        ></div>
      </div>
      <PdfViewer />
    </div>
  );
});
