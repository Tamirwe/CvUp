import { observer } from "mobx-react";
import { PdfViewer } from "../components/pdfViewer/PdfViewer";
import { CvsListWrapper } from "../components/cvs/CvsListWrapper";
import { Grid } from "@mui/material";
import { PositionsListWrapper } from "../components/positions/PositionsListWrapper";
import ReactQuill from "react-quill";
import "react-quill/dist/quill.snow.css";
import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { useStore } from "../Hooks/useStore";

export const Cv = observer(() => {
  let { id } = useParams();
  const { cvsStore } = useStore();

  let quillRef: any = null;
  let reactQuillRef: any = null;
  const [text, setText] = useState("");

  const handleChange = (html: string) => {
    setText(html);
  };

  const handleClick = () => {
    console.log(reactQuillRef.getEditor().getText());
  };
  // const [aware, setAwareness] = useState(null);
  useEffect(() => {
    (async () => {
      await cvsStore.getCv(id || "");
    })();

    attachQuillRefs();
    // const ydoc = new Y.Doc();
    // const provider = new WebrtcProvider("collab-demo-room", ydoc);
    // const ytext = ydoc.getText("quill");
    // const binding = new QuillBinding(ytext, quillRef, provider.awareness);
  }, [id]);

  const attachQuillRefs = () => {
    if (typeof reactQuillRef.getEditor !== "function") return;
    quillRef = reactQuillRef.getEditor();
    quillRef.format("direction", "rtl");
    quillRef.format("align", "right");
  };

  const modulesRef = {
    toolbar: [
      [{ header: [1, 2, false] }],
      ["bold", "italic", "underline", "strike", "link"],
      [
        { list: "ordered" },
        { list: "bullet" },
        { indent: "-1" },
        { indent: "+1" },
      ],
      // ["link"],
      // ["clean"],
      [{ direction: "rtl" }],
      ["save"],
    ],
  };

  const formats = [
    "font",
    "size",
    "bold",
    "italic",
    "underline",
    "strike",
    "color",
    "background",
    "script",
    "header",
    "blockquote",
    "code-block",
    "indent",
    "list",
    "direction",
    "align",
    "link",
    "image",
    "video",
    "formula",
  ];

  return (
    // <div className="cv-layout">
    //   <div className="columns">
    //     <div className="column">

    //     </div>
    //     <div className="column">
    //       <div style={{ position: "relative" }}>
    //         <button
    //           onClick={handleClick}
    //           style={{ position: "absolute", zIndex: 999, right: 0 }}
    //         >
    //           Save
    //         </button>{" "}
    <>
      <ReactQuill
        ref={(el) => {
          reactQuillRef = el;
        }}
        modules={modulesRef}
        theme={"snow"}
        formats={formats}
        value={text}
        onChange={handleChange}
      />
      <PdfViewer />
    </>
    //     </div>
    //     <div className="column">

    //     </div>
    //   </div>
    // </div>
  );
});
