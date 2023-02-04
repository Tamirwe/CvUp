import { observer } from "mobx-react";
import {
  Button,
  Card,
  CardActions,
  CardContent,
  Typography,
} from "@mui/material";
import ReactQuill from "react-quill";
import "react-quill/dist/quill.snow.css";
import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import Draggable, { DraggableData, DraggableEvent } from "react-draggable";
import { useStore } from "../../Hooks/useStore";
import { MdOutlineDragIndicator } from "react-icons/md";

export const CandReview = observer(() => {
  const { cvsStore } = useStore();
  const [x, setX] = useState(50);
  const [y, setY] = useState(50);

  useEffect(() => {
    setX(parseInt(localStorage.getItem("rteX") || "50"));
    setY(parseInt(localStorage.getItem("rteY") || "50"));
  }, []);

  const handleStop = (e: DraggableEvent, data: DraggableData) => {
    setX(data.x);
    setY(data.y);
    localStorage.setItem("rteX", data.x.toString());
    localStorage.setItem("rteY", data.y.toString());
  };

  let quillRef: any = null;
  let reactQuillRef: any = null;
  const [quillHtml, setQuillHtml] = useState(
    '<p class="ql-direction-rtl ql-align-right"><u>sdf sdf sfdf sdfsdf sdfsdf</u></p><p class="ql-align-right ql-direction-rtl">sdf</p><p class="ql-align-right ql-direction-rtl">sdf</p><p class="ql-align-right ql-direction-rtl"><br></p><p class="ql-align-right ql-direction-rtl">sdfsdfsdf <em>dsfsfd </em><strong>sdfhjgjkjhk </strong><u>jhlkjklljklj</u></p>'
  );

  const handleChange = (html: string) => {
    setQuillHtml(html);
  };

  useEffect(() => {
    if (reactQuillRef) {
      attachQuillRefs();
    }
  }, [reactQuillRef]); // eslint-disable-line react-hooks/exhaustive-deps

  const attachQuillRefs = () => {
    if (typeof reactQuillRef.getEditor !== "function") return;
    quillRef = reactQuillRef.getEditor();
    quillRef.format("direction", "rtl");
    quillRef.format("align", "right");
  };

  const handleCancel = () => {
    cvsStore.openCvReviewDialogOpen = false;
  };

  const handleSave = async () => {
    const reviewText = reactQuillRef.getEditor().getText();
    const reviewHtml = reactQuillRef.getEditor().root.innerHTML;
    await cvsStore.saveCvReview(reviewText, reviewHtml);
    cvsStore.openCvReviewDialogOpen = false;
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
    <div
      className="quill-rte"
      style={{ display: cvsStore.openCvReviewDialogOpen ? "block" : "none" }}
    >
      <Draggable handle="strong" onStop={handleStop} position={{ x: x, y: y }}>
        <Card sx={{ backgroundColor: "#f1f1f1" }}>
          <CardContent>
            <Typography variant="h5" component="div">
              <strong
                style={{
                  cursor: "move",
                  backgroundColor: "#d0e9ff",
                }}
              >
                <div
                  title="click to drag"
                  style={{
                    backgroundColor: "#d0e9ff",
                    marginBottom: 12,
                    display: "flex",
                  }}
                >
                  <div style={{ paddingTop: 6, color: "#a1a1a1" }}>
                    <MdOutlineDragIndicator />
                  </div>
                  <div
                    style={{ paddingTop: 3, paddingLeft: 13, color: "#524f4f" }}
                  >
                    Review
                  </div>
                </div>
              </strong>
            </Typography>
            <ReactQuill
              style={{ backgroundColor: "#fff" }}
              ref={(el) => {
                reactQuillRef = el;
              }}
              modules={modulesRef}
              theme={"snow"}
              formats={formats}
              value={quillHtml}
              onChange={handleChange}
            />
          </CardContent>
          <CardActions>
            <Button
              color="secondary"
              sx={{ marginLeft: "auto" }}
              onClick={handleCancel}
              size="small"
            >
              Cancel
            </Button>
            <Button sx={{ marginLeft: 25 }} onClick={handleSave} size="small">
              Save
            </Button>
          </CardActions>
        </Card>
      </Draggable>
    </div>
  );
});
