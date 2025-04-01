import { Dialog, DialogContent, IconButton } from "@mui/material";
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
  const [review1, setReview1] = useState("");
  const [cand1, setCand1] = useState<IRestoreCandDetails>();
  const [review2, setReview2] = useState("");
  const [cand2, setCand2] = useState<IRestoreCandDetails>();

  useEffect(() => {
    const lastReviewCandDetails: IRestoreCandDetails = JSON.parse(
      localStorage.getItem("LastReviewCandDetails") || '{"candidateId":0}'
    );

    setCand1(lastReviewCandDetails);

    const lastReview = localStorage.getItem("LastReview") || "0";
    setReview1(lastReview);

    const prevReviewCandDetails: IRestoreCandDetails = JSON.parse(
      localStorage.getItem("PrevReviewCandDetails") || '{"candidateId":0}'
    );
    setCand2(prevReviewCandDetails);

    const prevReview = localStorage.getItem("PrevReview") || "0";
    setReview2(prevReview);
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
    >
      <BootstrapDialogTitle id="dialog-title" onClose={() => onClose(false)}>
        Restore Candidate Review
      </BootstrapDialogTitle>
      <DialogContent sx={{ overflow: "hidden", paddingBottom: "15px" }}>
        {cand1?.candId! > 0 || cand2?.candId! > 0 ? (
          <div
            style={{
              direction: "rtl",
            }}
          >
            <div>
              {review1 && (
                <div>
                  {cand1?.firstName && (
                    <div
                      style={{
                        fontSize: "1rem",
                        fontWeight: "bold",
                        color: "gray",
                      }}
                    >{`${cand1?.firstName} ${cand1?.lastName}${
                      cand1?.customerName ? " - " + cand1?.customerName : ""
                    }${
                      cand1?.positionName ? " - " + cand1?.positionName : ""
                    }`}</div>
                  )}
                  <div
                    style={{
                      maxHeight: "20rem",
                      overflow: "auto",
                      padding: "5px",
                    }}
                  >
                    <IconButton
                      title="Copy Text"
                      sx={{ fontSize: "1.54rem" }}
                      size="small"
                      onClick={() => navigator.clipboard.writeText(review1)}
                    >
                      <CiUsb />
                    </IconButton>

                    <pre
                      style={{
                        whiteSpace: "break-spaces",
                        direction: "rtl",
                        textAlign: "right",
                        fontFamily: "inherit",
                        margin: 0,
                      }}
                      dangerouslySetInnerHTML={{
                        __html: review1 || "",
                      }}
                    ></pre>
                  </div>
                </div>
              )}
            </div>
            <div style={{ padding: "2rem", textAlign: "center" }}>
              ------------------------------------------------------
            </div>
            <div style={{ paddingBottom: "1rem" }}>
              {review2 && (
                <div>
                  {cand2?.firstName && (
                    <div
                      style={{
                        fontSize: "1rem",
                        fontWeight: "bold",
                        color: "gray",
                      }}
                    >{`${cand2?.firstName} ${cand2?.lastName}${
                      cand2?.customerName ? " - " + cand2?.customerName : ""
                    }${
                      cand2?.positionName ? " - " + cand2?.positionName : ""
                    }`}</div>
                  )}
                  <div
                    style={{
                      maxHeight: "20rem",
                      overflow: "auto",
                      padding: "5px",
                    }}
                  >
                    <IconButton
                      title="Copy Text"
                      sx={{ fontSize: "1.54rem" }}
                      size="small"
                      onClick={() => navigator.clipboard.writeText(review2)}
                    >
                      <CiUsb />
                    </IconButton>

                    <pre
                      style={{
                        whiteSpace: "break-spaces",
                        direction: "rtl",
                        textAlign: "right",
                        fontFamily: "inherit",
                        margin: 0,
                      }}
                      dangerouslySetInnerHTML={{
                        __html: review2 || "",
                      }}
                    ></pre>
                  </div>
                </div>
              )}
            </div>
          </div>
        ) : (
          <div style={{ fontSize: "2rem", padding: "4rem" }}>No data yet.</div>
        )}
      </DialogContent>
    </Dialog>
  );
};
