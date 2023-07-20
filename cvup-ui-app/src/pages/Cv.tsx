import { observer } from "mobx-react";
import { PdfViewer } from "../components/pdfViewer/PdfViewer";
import "react-quill/dist/quill.snow.css";
import { useStore } from "../Hooks/useStore";
import styles from "./Cv.module.scss";

export const Cv = observer(() => {
  const { candsStore, authStore } = useStore();

  return (
    <div className={styles.scrollCv}>
      <div className="qlCustom">
        <div
          style={{
            direction: authStore.isRtl ? "rtl" : "ltr",
            textAlign: authStore.isRtl ? "right" : "left",
          }}
          dangerouslySetInnerHTML={{
            __html: candsStore.candAllSelected?.review || "",
          }}
        ></div>
      </div>
      <PdfViewer />
    </div>
  );
});
