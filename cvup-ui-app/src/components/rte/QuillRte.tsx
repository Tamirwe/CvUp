import { observer } from "mobx-react";
import ReactQuill from "react-quill";
import "react-quill/dist/quill.snow.css";
import { useEffect, useState } from "react";

interface IProps {
  onInit: (quillRef: any) => void;
  quillHtml?: string;
}

export const QuillRte = observer(({ onInit, quillHtml }: IProps) => {
  const [quillRef, setQuillRef] = useState<any>(null);

  // let quillRef: any = null;
  // let reactQuillRef: any = null;
  // const [quillHtml, setQuillHtml] = useState("");

  // useEffect(() => {
  //   setQuillHtml(candsStore.candSelected?.review || "");
  // }, []); // eslint-disable-line react-hooks/exhaustive-deps

  useEffect(() => {
    if (quillRef) {
      initQuillEditor();
    }
  }, [quillRef]); // eslint-disable-line react-hooks/exhaustive-deps

  const initQuillEditor = () => {
    if (typeof quillRef.getEditor !== "function") return;
    const quillEditor = quillRef.getEditor();
    quillEditor.format("direction", "rtl");
    quillEditor.format("align", "right");
    onInit(quillEditor);
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
    <ReactQuill
      style={{ backgroundColor: "#fff" }}
      ref={(el) => {
        setQuillRef(el);
      }}
      modules={modulesRef}
      theme={"snow"}
      formats={formats}
      value={quillHtml}
      // onChange={handleChange}
    />
  );
});
