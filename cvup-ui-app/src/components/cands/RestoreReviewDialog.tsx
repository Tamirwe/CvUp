import {
  Dialog,
  DialogContent,
  IconButton,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
} from "@mui/material";
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
        <TableContainer sx={{ direction: "rtl" }}>
            <Table size="small">
              <TableHead>
                <TableRow>
                  <TableCell align="right">Name</TableCell>
                  <TableCell align="right">Date</TableCell>
                  <TableCell align="right">Review</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {history.map((cand, index) => (
                  <TableRow key={index}>
                    <TableCell
                      align="right"
                      sx={{ whiteSpace: "nowrap", verticalAlign: "top" }}
                    >
                      {cand.firstName} {cand.lastName}
                    </TableCell>
                    <TableCell
                      align="right"
                      sx={{ whiteSpace: "nowrap", verticalAlign: "top" }}
                    >
                      {cand.updatedDateTime &&
                        new Date(cand.updatedDateTime).toLocaleString()}
                    </TableCell>
                    <TableCell align="right" sx={{ verticalAlign: "top" }}>
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
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
        </TableContainer>
      </DialogContent>
    </Dialog>
  );
};
