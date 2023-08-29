import {
  Box,
  Collapse,
  IconButton,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Stack,
} from "@mui/material";
import { format } from "date-fns";
import { observer } from "mobx-react-lite";
import { useCallback, useEffect, useRef, useState } from "react";
import {
  MdExpandLess,
  MdExpandMore,
  MdHistory,
  MdOutlineFilterNone,
} from "react-icons/md";
import { useLocation, useNavigate } from "react-router-dom";
import { useStore } from "../../Hooks/useStore";
import { CandsSourceEnum } from "../../models/GeneralEnums";
import { ICand } from "../../models/GeneralModels";
import { CandDupCvsList } from "./CandDupCvsList";
import { CandsPosStagesList } from "./CandsPosStagesList";
import { isMobile } from "react-device-detect";
import styles from "./CandsList.module.scss";
import classNames from "classnames";

interface IProps {
  candsListData: ICand[];
  candsSource: CandsSourceEnum;
  advancedOpen?: boolean;
}

export const CandsList = observer(
  ({ candsListData, candsSource, advancedOpen }: IProps) => {
    const { candsStore, generalStore, positionsStore } = useStore();
    let location = useLocation();
    const navigate = useNavigate();

    const listRef = useRef<any>(null);
    // const [dupOpenCandId, setDupOpenCandId] = useState(0);
    const [listCands, setListCands] = useState<ICand[]>([]);

    useEffect(() => {
      if (candsListData) {
        setListCands([
          ...candsListData?.slice(0, Math.max(listCands.length, 50)),
        ]);
      }

      //setDupOpenCandId(0);
    }, [candsListData]); // eslint-disable-line react-hooks/exhaustive-deps

    // useEffect(() => {
    //   if (!candsStore.candDisplay) {
    //     setDupOpenCandId(0);
    //   }
    // }, [candsStore.candDisplay]); // eslint-disable-line react-hooks/exhaustive-deps

    useEffect(() => {
      setTimeout(() => {
        listRef.current.scrollTop = 0;
      }, 200);
    }, [positionsStore.selectedPosition]); // eslint-disable-line react-hooks/exhaustive-deps

    const onScroll = useCallback(() => {
      const instance = listRef.current;

      if (
        instance.scrollHeight - instance.clientHeight <
        instance.scrollTop + 50
      ) {
        if (listCands) {
          const numRecords = listCands.length;

          const newPosList = listCands.concat(
            candsListData?.slice(numRecords, numRecords + 50)
          );

          setListCands(newPosList);
        }

        // console.log(instance.scrollTop);
      }
    }, [listCands, setListCands]);

    useEffect(() => {
      const instance = listRef.current;

      instance.addEventListener("scroll", onScroll);

      return () => {
        instance.removeEventListener("scroll", onScroll);
      };
    }, [onScroll]);

    return (
      <List
        ref={listRef}
        dense={true}
        sx={{
          height: isMobile
            ? advancedOpen
              ? "calc(100vh - 205px)"
              : "70vh"
            : advancedOpen
            ? "calc(100vh - 173px)"
            : "calc(100vh - 125px)",
        }}
        className={classNames({
          [styles.candList]: true,
          [styles.isMobile]: isMobile,
        })}
      >
        {listCands.map((cand, i) => {
          const posStage = cand.posStages?.find(
            (x) => x._pid === positionsStore.selectedPosition?.id
          );
          return (
            <ListItem
              key={`${cand.candidateId}${i}`}
              dense
              disablePadding
              component="nav"
              sx={{
                flexDirection: "column",
                alignItems: "normal",
                direction: "rtl",
                pl: "2px",
              }}
            >
              <ListItemButton
                sx={{
                  pl: 0,
                  fontWeight: "bold",
                  color: posStage
                    ? candsStore.findStageColor(posStage._tp)
                    : cand.isSeen
                    ? "unset"
                    : "green",
                }}
                selected={
                  cand.candidateId === candsStore.candDisplay?.candidateId
                }
                onClick={async (event) => {
                  event.stopPropagation();
                  event.preventDefault();

                  if (location.pathname !== "/cv") {
                    navigate(`/cv`);
                  }

                  await candsStore.displayCv(cand, candsSource);

                  if (isMobile) {
                    generalStore.rightDrawerOpen = false;
                  }
                }}
              >
                <Box
                  sx={{
                    width: "100%",
                  }}
                >
                  <div
                    style={{
                      display: "flex",
                      justifyContent: "space-between",
                      alignItems: "center",
                    }}
                  >
                    <div
                      title={
                        candsSource === CandsSourceEnum.AllCands
                          ? "Email subject"
                          : ""
                      }
                      className={classNames({
                        [styles.listItemText]: true,
                        [styles.isMobile]: isMobile,
                      })}
                    >
                      {candsSource === CandsSourceEnum.AllCands
                        ? cand.emailSubject
                            .replace("מועמדות חדשה מ", "")
                            .replace(" למשרת ", " - ")
                        : `${cand.firstName || ""} ${cand.lastName || ""}`}
                    </div>

                    <div
                      style={{
                        display: "flex",
                        alignItems: "center",
                      }}
                    >
                      <div
                        title="Cv received"
                        className={classNames({
                          [styles.listItemDate]: true,
                          [styles.isMobile]: isMobile,
                        })}
                      >
                        {posStage ? (
                          <div style={{ width: "100%", textAlign: "right" }}>
                            {` ${format(
                              new Date(posStage._dt),
                              "MMM d, yyyy"
                            )} - ${candsStore.findStageName(posStage._tp)} `}
                          </div>
                        ) : (
                          format(new Date(cand.cvSent), "MMM d, yy")
                        )}
                      </div>
                      {candsSource === CandsSourceEnum.Position ? (
                        <ListItemIcon
                          title="Has duplicates cv`s"
                          sx={{
                            minWidth: 0,
                            visibility: "hidden",
                          }}
                        >
                          <IconButton
                            color="primary"
                            sx={{ fontSize: "0.8rem", paddingLeft: "1px" }}
                          >
                            <MdOutlineFilterNone />
                          </IconButton>
                        </ListItemIcon>
                      ) : (
                        <>
                          <ListItemIcon
                            title="Has duplicates cv`s"
                            sx={{
                              minWidth: 0,
                              visibility: cand.hasDuplicates
                                ? "visible"
                                : "hidden",
                            }}
                          >
                            <IconButton
                              color="primary"
                              sx={{ fontSize: "0.8rem", paddingLeft: "1px" }}
                            >
                              <MdOutlineFilterNone />
                            </IconButton>
                          </ListItemIcon>
                          <ListItemIcon
                            title="Has history"
                            sx={{
                              minWidth: 0,
                              visibility:
                                cand.posStages && cand.posStages?.length > 0
                                  ? "visible"
                                  : "hidden",
                            }}
                          >
                            <IconButton
                              color="primary"
                              sx={{ fontSize: "0.8rem", paddingLeft: "1px" }}
                            >
                              <MdHistory />
                            </IconButton>
                          </ListItemIcon>
                        </>
                      )}
                      {/* <ListItemIcon
                        sx={{
                          visibility:
                            cand.posStages && cand.posStages?.length > 0
                              ? "visible"
                              : "hidden",
                        }}
                        onClick={async (event) => {
                          event.stopPropagation();
                          event.preventDefault();

                          if (location.pathname !== "/cv") {
                            navigate(`/cv`);
                          }

                          await candsStore.displayCv(cand, candsSource);
                          // await candsStore.getDuplicatesCvsList(cand);

                          if (dupOpenCandId !== cand.candidateId) {
                            setDupOpenCandId(cand.candidateId);
                          } else {
                            setDupOpenCandId(0);
                          }
                        }}
                      >
                        <IconButton
                          color="primary"
                          aria-label="upload picture"
                          component="label"
                        >
                          {dupOpenCandId === cand.candidateId ? (
                            <MdExpandLess />
                          ) : (
                            <MdExpandMore />
                          )}
                        </IconButton>
                      </ListItemIcon> */}
                    </div>
                  </div>
                  {cand.city && (
                    <div
                      className={classNames({
                        [styles.listItemCity]: true,
                        [styles.isMobile]: isMobile,
                      })}
                    >
                      {cand.city}
                    </div>
                  )}

                  {/* {candsSource !== CandsSourceEnum.Position &&
                    cand.posStages &&
                    cand.posStages?.length > 0 && (
                      <CandsPosStagesList
                        cand={cand}
                        candsSource={candsSource}
                      />
                    )} */}
                </Box>
              </ListItemButton>
              {/* {dupOpenCandId > 0 && (
                <Collapse
                  in={dupOpenCandId === cand.candidateId}
                  timeout="auto"
                  unmountOnExit
                > */}
              {/* {candsSource === CandsSourceEnum.Position &&
                    cand.posStages &&
                    cand.posStages?.length > 0 && ( */}
              {/* <div style={{ padding: " 0.2rem 0.75rem 0.75rem 0.75rem" }}> */}
              {/* <div style={{ fontWeight: 700, padding: "0.2rem" }}>
                          Candidate Positions
                        </div> */}
              {/* <CandsPosStagesList cand={cand} candsSource={candsSource} />
                  </div> */}
              {/* )} */}
              {/* <CandDupCvsList candPosCvId={cand.posCvId} /> */}
              {/* </Collapse>
              )} */}
            </ListItem>
          );
        })}
      </List>
    );
  }
);
