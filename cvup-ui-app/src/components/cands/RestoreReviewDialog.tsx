import { Dialog, DialogContent, IconButton, Paper, Stack } from "@mui/material";
import { useEffect, useState } from "react";
import { BootstrapDialogTitle } from "../dialog/BootstrapDialogTitle";
import { IRestoreCandDetails } from "../../models/GeneralModels";
import { CiUsb } from "react-icons/ci";

interface IProps {
  isOpen: boolean;
  onClose: (isSaved: boolean) => void;
}

export const RestoreReviewDialog = ({ isOpen, onClose }: IProps) => {
  const [open, setOpen] = useState(false);
  const [history, setHistory] = useState<IRestoreCandDetails[]>([]);

  useEffect(() => {
    const reviewHistory: IRestoreCandDetails[] = JSON.parse(
      localStorage.getItem("ReviewHistory") || "[]"
    );

    setHistory(reviewHistory);
  }, []);

  useEffect(() => {
    setOpen(isOpen);
  }, [isOpen]);

  return (
    <Dialog
      open={open}
      fullWidth
      maxWidth={"md"}
      onClose={() => onClose(false)}
      PaperProps={{ sx: { minHeight: "30vh" } }}
    >
      <BootstrapDialogTitle id="dialog-title" onClose={() => onClose(false)}>
        Restore Candidate Review
      </BootstrapDialogTitle>
      <DialogContent sx={{ overflow: "hidden", paddingBottom: "15px" }}>
        <Stack spacing={1.5} sx={{ direction: "rtl" }}>
          {history.map((cand, index) => (
            <Paper key={index} variant="outlined" sx={{ padding: "0.75rem" }}>
              <Stack
                direction="row"
                flexWrap="wrap"
                justifyContent="space-between"
                alignItems="center"
                gap={1}
              >
                <span style={{ fontWeight: 700, color: "#7b84ff" }}>
                  {cand.firstName} {cand.lastName}
                </span>
                <Stack direction="row" alignItems="center" gap={0.5}>
                  {cand.updatedDateTime && (
                    <span
                      style={{
                        fontSize: "0.8rem",
                        color: "gray",
                        fontWeight: 700,
                      }}
                    >
                      {new Date(cand.updatedDateTime).toLocaleString()}
                    </span>
                  )}
                  <IconButton
                    title="Copy Text"
                    sx={{ fontSize: "1.54rem" }}
                    size="small"
                    onClick={() =>
                      navigator.clipboard.writeText(cand.review)
                    }
                  >
                    <CiUsb />
                  </IconButton>
                </Stack>
              </Stack>

              <div
                style={{
                  maxHeight: "12rem",
                  overflow: "auto",
                  padding: "5px",
                }}
              >
                <pre
                  style={{
                    whiteSpace: "break-spaces",
                    direction: "rtl",
                    textAlign: "right",
                    fontFamily: "inherit",
                    margin: 0,
                  }}
                  dangerouslySetInnerHTML={{
                    __html: cand.review || "",
                  }}
                ></pre>
              </div>
            </Paper>
          ))}
        </Stack>
      </DialogContent>
    </Dialog>
  );
};
