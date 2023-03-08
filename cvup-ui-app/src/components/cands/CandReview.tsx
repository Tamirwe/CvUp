import { observer } from "mobx-react";
import {
  Button,
  Card,
  CardActions,
  CardContent,
  Typography,
} from "@mui/material";
import "react-quill/dist/quill.snow.css";
import { useEffect, useState } from "react";
import Draggable, { DraggableData, DraggableEvent } from "react-draggable";
import { useStore } from "../../Hooks/useStore";
import { MdOutlineDragIndicator } from "react-icons/md";
import { QuillRte } from "../rte/QuillRte";

export const CandReview = observer(() => {
  const { candsStore, generalStore } = useStore();
  const [x, setX] = useState(50);
  const [y, setY] = useState(50);
  const [quillEditor, setQuillEditor] = useState<any>(null);

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

  const handleRteInit = (editor: any) => {
    setQuillEditor(editor);
  };

  const handleCancel = () => {
    generalStore.cvReviewDialogOpen = false;
  };

  const handleSave = async () => {
    const reviewText = quillEditor.getText();
    const reviewHtml = quillEditor.root.innerHTML;
    await candsStore.saveCvReview(reviewText, reviewHtml);
    generalStore.cvReviewDialogOpen = false;
  };

  return (
    <Draggable handle="strong" onStop={handleStop} position={{ x: x, y: y }}>
      <Card
        sx={{
          backgroundColor: "#f1f1f1",
          width: "40rem",
          zIndex: 9999,
          position: "fixed",
        }}
      >
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
          <QuillRte
            onInit={handleRteInit}
            quillHtml={candsStore.candAllSelected?.review}
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
  );
});
