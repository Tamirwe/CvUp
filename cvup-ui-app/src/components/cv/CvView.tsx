import { observer } from "mobx-react";
import { PdfViewer } from "../../components/pdfViewer/PdfViewer";
import "react-quill/dist/quill.snow.css";
import { useStore } from "../../Hooks/useStore";
import styles from "./CvView.module.scss";
import {
  Button,
  Grid,
  IconButton,
  Link,
  Paper,
  Stack,
  TextField,
} from "@mui/material";
import { CandsSourceEnum, EmailTypeEnum } from "../../models/GeneralEnums";
import { useCallback, useEffect, useRef, useState } from "react";
import { format } from "date-fns";
import { isMobile } from "react-device-detect";
import {
  MdContentCopy,
  MdKeyboardArrowDown,
  MdKeyboardArrowUp,
} from "react-icons/md";
import { copyToClipBoard } from "../../utils/GeneralUtils";
import { CandsPosStagesList } from "../cands/CandsPosStagesList";
import { CandDupCvsList } from "../cands/CandDupCvsList";
import { useLocation, useNavigate } from "react-router-dom";
import { PosStages } from "./PosStages";
import useDebounce from "../../Hooks/useDebounce";

export const CvView = observer(() => {
  const { candsStore, authStore, generalStore, positionsStore } = useStore();
  const navigate = useNavigate();
  let location = useLocation();
  const [candidateName, setCandidateName] = useState("");
  const [review, setReview] = useState("");
  const debouncedValue = useDebounce<string>(review, 2000);
  const [isLoaded, setIsLoaded] = useState(false);

  const scrollRef = useRef<any>(null);

  useEffect(() => {
    if (isLoaded) {
      candsStore.saveReviewToLocalStorage(review);
    }
  }, [debouncedValue]);

  useEffect(() => {
    scrollRef.current.scrollTop = 0;

    if (candsStore.candDisplay) {
      candsStore.candReviewSync = "";
      setIsLoaded(false);
      getCandName();
      generalStore.showReviewCandDialog = false;
      setReview(candsStore.candDisplay?.review || "");
      candsStore.getDuplicatesCvsList(candsStore.candDisplay);
    }
  }, [candsStore.candDisplay]);

  useEffect(() => {
    if (candsStore.candDisplay) {
      const posCand = candsStore.posCandsList.find(
        (x) => x.candidateId === candsStore.candDisplay?.candidateId
      );

      if (posCand) {
        candsStore.displayCv(posCand, CandsSourceEnum.Position);
      }
    }
  }, [candsStore.posCandsList]);

  const getCandName = useCallback(() => {
    let fullName = `${candsStore.candDisplay?.firstName || ""} ${
      candsStore.candDisplay?.lastName || ""
    }`;

    if (!fullName.trim()) {
      fullName = "Name not found";
    }

    setCandidateName(fullName);
  }, []);

  const handlePositionClick = useCallback(
    async (event: React.MouseEvent<HTMLButtonElement | HTMLAnchorElement>) => {
      await positionsStore.positionClick(
        positionsStore.candDisplayPosition!.id,
        true
      );
      candsStore.setDisplayCandOntopPCList();
    },
    []
  );

  useEffect(() => {
    if (generalStore.showInterviewFullDialog) {
      candsStore.candReviewSync = review;
    } else if (candsStore.candReviewSync !== "") {
      setReview(candsStore.candReviewSync);
    }
  }, [generalStore.showInterviewFullDialog]);

  return (
    <Paper elevation={3} sx={{ margin: "0rem" }}>
      <div
        ref={scrollRef}
        className={styles.scrollCv}
        style={{
          marginTop: "0.5rem",
          overflow:
            isMobile &&
            (generalStore.rightDrawerOpen || generalStore.leftDrawerOpen)
              ? "hidden"
              : "scroll",
        }}
      >
        {candsStore.candDisplay && (
          <Grid
            container
            sx={{
              direction: "rtl",
              padding: "1rem 1.5rem 1rem 1rem",
            }}
          >
            <Grid item xs={12} lg={6}>
              <Grid container>
                <Grid
                  item
                  xs={12}
                  sx={{
                    color: "#7b84ff",
                    fontWeight: 700,
                    fontSize: isMobile ? "0.9rem" : "1.1rem",
                  }}
                >
                  {positionsStore.candDisplayPosition && (
                    <Link href="#" onClick={handlePositionClick}>
                      {positionsStore.candDisplayPosition?.name || ""}
                      &nbsp;-&nbsp;
                      {positionsStore.candDisplayPosition?.customerName || ""}
                    </Link>
                  )}
                </Grid>
                <Grid
                  item
                  xs={12}
                  sx={{
                    display: "flex",
                    flexDirection: "row",
                    gap: "1rem",
                    paddingTop: "1rem",
                  }}
                >
                  <Grid
                    container
                    sx={{
                      gap: "1rem",
                    }}
                  >
                    <Grid item>
                      <Link
                        sx={{ whiteSpace: "nowrap" }}
                        href="#"
                        onClick={() => {
                          generalStore.showCandFormDialog = true;
                        }}
                      >
                        {candidateName}{" "}
                        {candsStore.candDisplay?.city && (
                          <span style={{ color: "gray" }}>
                            {" - "} {candsStore.candDisplay?.city}
                          </span>
                        )}
                      </Link>
                    </Grid>
                    <Grid item>
                      <Link
                        href="#"
                        onClick={() => {
                          generalStore.showEmailDialog =
                            EmailTypeEnum.Candidate;
                        }}
                      >
                        {candsStore.candDisplay?.email}{" "}
                      </Link>
                      &nbsp;
                      {!isMobile && (
                        <Link
                          title="Copy email"
                          href="#"
                          onClick={async () => {
                            await copyToClipBoard(
                              candsStore.candDisplay?.email || ""
                            );
                          }}
                        >
                          <MdContentCopy />
                        </Link>
                      )}
                    </Grid>
                    <Grid item>
                      <a href={"tel:" + candsStore.candDisplay?.phone}>
                        {candsStore.candDisplay?.phone}
                      </a>
                    </Grid>
                  </Grid>
                </Grid>
              </Grid>
            </Grid>
            <Grid item xs={12} lg={6}>
              <PosStages />
            </Grid>
            <Grid item xs={12} lg={12}>
              {candsStore.candDisplay?.allCustomersReviews && (
                <div>
                  <div
                    style={{
                      padding: "1.5rem 0 0.2rem 0",
                    }}
                  >
                    <Link
                      title="Review updated"
                      sx={{ whiteSpace: "nowrap" }}
                      href="#"
                      onClick={() => {
                        generalStore.showCustomerReviewCandDialog =
                          !generalStore.showCustomerReviewCandDialog;
                      }}
                    >
                      Customers Reviews
                    </Link>
                  </div>

                  {candsStore.candDisplay?.allCustomersReviews?.map(
                    (rev, i) => {
                      return (
                        <div key={i} style={{ paddingBottom: "0.432rem" }}>
                          <div
                            style={{
                              display: "flex",
                              flexFlow: "wrap",
                            }}
                          >
                            <div
                              className={styles.custReviewTitle}
                              style={{ minWidth: "5.5rem" }}
                            >
                              {format(
                                new Date(rev.updated || ""),
                                "MMM d, yyyy"
                              )}{" "}
                            </div>
                            <div className={styles.custReviewTitle}>
                              {rev.posName} -&nbsp;
                            </div>
                            <div className={styles.custReviewTitle}>
                              {rev.custName} :&nbsp;
                            </div>
                            <div style={{ paddingRight: "0.53rem" }}>
                              {rev.review}
                            </div>
                          </div>
                        </div>
                      );
                    }
                  )}
                </div>
              )}
            </Grid>

            <Grid item xs={12} lg={12}>
              {candsStore.candDisplay?.review &&
                candsStore.candDisplay?.reviewDate && (
                  <div
                    title="Review updated date"
                    style={{
                      display: "flex",
                      flexDirection: "row-reverse",
                      fontSize: "0.75rem",
                      paddingTop: "1rem",
                    }}
                  >
                    <Link
                      title="Review updated"
                      sx={{ whiteSpace: "nowrap" }}
                      href="#"
                      onClick={() => {
                        generalStore.showReviewCandDialog =
                          !generalStore.showReviewCandDialog;
                      }}
                    >
                      <span style={{ direction: "ltr" }}></span>
                      {format(
                        new Date(candsStore.candDisplay?.reviewDate),
                        "MMM d, yyyy"
                      )}{" "}
                    </Link>
                  </div>
                )}
              <div>
                {generalStore.showReviewCandDialog && !isMobile ? (
                  <Grid container justifyContent="space-between">
                    <Grid item xs={12}>
                      <TextField
                        sx={{
                          direction: "rtl",
                          //"& .MuiInputBase-input": { maxHeight: "calc(100% - 21rem)" },
                        }}
                        fullWidth
                        multiline
                        rows={17}
                        margin="normal"
                        type="text"
                        id="description"
                        label="Description"
                        variant="outlined"
                        onChange={(e) => {
                          setReview(e.target.value);
                          !isLoaded && setIsLoaded(true);
                        }}
                        value={review}
                      />
                    </Grid>
                    <Grid item xs={12}>
                      <Stack direction="row" justifyContent="flex-end" gap={1}>
                        <Button
                          color="secondary"
                          onClick={() => {
                            setReview(candsStore.candDisplay?.review || "");
                            generalStore.showReviewCandDialog = false;
                          }}
                        >
                          Cancel
                        </Button>

                        <Button
                          color="secondary"
                          onClick={async () => {
                            const res = await candsStore.saveCandReview(review);
                          }}
                        >
                          Save
                        </Button>
                      </Stack>
                    </Grid>
                  </Grid>
                ) : (
                  <pre
                    onClick={() => {
                      generalStore.showReviewCandDialog =
                        !generalStore.showReviewCandDialog;
                    }}
                    style={{
                      whiteSpace: "break-spaces",
                      direction: authStore.isRtl ? "rtl" : "ltr",
                      textAlign: authStore.isRtl ? "right" : "left",
                      fontFamily: "inherit",
                      margin: 0,
                    }}
                    dangerouslySetInnerHTML={{
                      __html: candsStore.candDisplay?.review || "",
                    }}
                  ></pre>
                )}
              </div>
            </Grid>

            <Grid item xs={12} lg={12}>
              <Grid container>
                {candsStore.candDisplay.posStages &&
                candsStore.candDisplay.posStages?.length > 0 ? (
                  <Grid item xs={12} lg={6} pl={2}>
                    <div
                      style={{
                        padding: "1.5rem 0 0.2rem 0",
                        color: "#149bed",
                        fontSize: "1rem",
                        fontWeight: 500,
                      }}
                    >
                      History
                    </div>
                    <div
                      style={{
                        border: "1px solid #ffdcdc",
                        padding: "0.5rem 1rem 1rem 1rem",
                      }}
                    >
                      <CandsPosStagesList
                        cand={candsStore.candDisplay}
                        candsSource={CandsSourceEnum.AllCands}
                      />
                    </div>
                  </Grid>
                ) : (
                  <Grid item xs={12} lg={6} pl={2}></Grid>
                )}
                <Grid item xs={12} lg={6}>
                  <div
                    style={{
                      padding: "1.5rem 0 0.2rem 0",
                      color: "#149bed",
                      fontSize: "1rem",
                      fontWeight: 500,
                    }}
                  >
                    Duplicates cv`s
                  </div>
                  <CandDupCvsList
                    candPosCvId={candsStore.candDisplay?.posCvId}
                  />
                </Grid>
              </Grid>
            </Grid>
          </Grid>
        )}
        <PdfViewer />
        <br />
        <br />
        <br />
      </div>
      {isMobile && candsStore.candDisplay && (
        <div style={{ position: "fixed", bottom: "1rem", right: "0.1rem" }}>
          <IconButton
            title="Next"
            sx={{ display: "block", fontSize: "3rem", color: "#b700ff" }}
            size="medium"
            onClick={async (event) => {
              event.stopPropagation();
              event.preventDefault();

              if (location.pathname !== "/cv") {
                navigate(`/cv`);
              }

              await candsStore.nexePrevCv(-1);
            }}
          >
            <MdKeyboardArrowUp />
          </IconButton>
          <IconButton
            title="Previous"
            sx={{ display: "block", fontSize: "3rem", color: "#b700ff" }}
            size="medium"
            onClick={async (event) => {
              event.stopPropagation();
              event.preventDefault();

              if (location.pathname !== "/cv") {
                navigate(`/cv`);
              }

              await candsStore.nexePrevCv(1);
            }}
          >
            <MdKeyboardArrowDown />
          </IconButton>
        </div>
      )}
    </Paper>
  );
});
