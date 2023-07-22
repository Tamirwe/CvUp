import { observer } from "mobx-react";
import ReactQuill, { Quill } from "react-quill";
import "react-quill/dist/quill.snow.css";
import { forwardRef, useEffect, useState } from "react";
import "./quillStyles.css";

interface IProps {
  quillHtml?: string;
}

Quill.register(Quill.import("attributors/style/direction"), true);
Quill.register(Quill.import("attributors/style/align"), true);

const Size = Quill.import("attributors/style/size");
Size.whitelist = ["0.75em", "1em", "1.5em", "2.5em"];
Quill.register(Size, true);

const Parchment = Quill.import("parchment");
class IndentAttributor extends Parchment.Attributor.Style {
  constructor(a: any, b: any, c: any) {
    super(a, b, c);
  }

  add(node: any, value: any) {
    if (value === 0) {
      this.remove(node);
      return true;
    } else {
      return super.add(node, `${value}em`);
    }
  }
}

let IndentStyle = new IndentAttributor("indent", "text-indent", {
  scope: Parchment.Scope.BLOCK,
  whitelist: ["1em", "2em", "3em", "4em", "5em", "6em", "7em", "8em", "9em"],
});

Quill.register(IndentStyle, true);

export const QuillRte = observer(
  forwardRef(({ quillHtml }: IProps, refQuill: any) => {
    // const [quillRef, setQuillRef] = useState<any>(null);

    // let quillRef: any = null;
    // let reactQuillRef: any = null;
    // const [quillHtml, setQuillHtml] = useState("");

    // useEffect(() => {
    //   setQuillHtml(candsStore.candSelected?.review || "");
    // }, []); // eslint-disable-line react-hooks/exhaustive-deps

    // useEffect(() => {
    //   if (quillRef) {
    //     initQuillEditor();
    //   }
    // }, [quillRef]); // eslint-disable-line react-hooks/exhaustive-deps

    // const initQuillEditor = () => {
    //   if (typeof quillRef.getEditor !== "function") return;
    //   const quillEditor = quillRef.getEditor();
    //   quillEditor.format("direction", "rtl");
    //   quillEditor.format("align", "right");
    //   onInit(quillEditor);
    // };
    const [Value, setValue] = useState("");
    const [ReadOnly, setReadOnly] = useState(true);

    useEffect(() => {
      setValue(quillHtml || "");
      if (quillHtml != null) {
        setReadOnly(false);
      }
    }, [quillHtml]);

    const modules = {
      toolbar: [
        // [{ header: [1, 2, false] }],
        [{ size: ["0.75em", "1em", "1.5em", "2.5em"] }],

        ["bold", "italic", "underline", "strike", "link"],
        [
          { list: "ordered" },
          { list: "bullet" },
          { indent: "-1" },
          { indent: "+1" },
        ],
        [{ color: [] }],
        [{ background: [] }],
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
      "indent",
      "list",
      "direction",
      "align",
      "link",
      // "image",
      // "video",
      // "formula",
      // "script",
      // "header",
      // "blockquote",
      // "code-block",
    ];

    return (
      <ReactQuill
        style={{ backgroundColor: "#fff" }}
        ref={(el) => {
          if (el) {
            const quillEditor = el.getEditor();
            refQuill.current = quillEditor;
            quillEditor.format("direction", "rtl");
            quillEditor.format("align", "right");
          }
        }}
        modules={modules}
        theme={"snow"}
        formats={formats}
        value={Value}
        readOnly={ReadOnly}
        // onChange={handleChange}
      />
    );
  })
);
