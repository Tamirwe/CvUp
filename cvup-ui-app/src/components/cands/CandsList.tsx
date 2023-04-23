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
import { MdExpandLess, MdExpandMore, MdRemove } from "react-icons/md";
import { useLocation, useNavigate } from "react-router-dom";
import { useStore } from "../../Hooks/useStore";
import { CandsSourceEnum, TabsCandsEnum } from "../../models/GeneralEnums";
import { ICand } from "../../models/GeneralModels";
import { CandDupCvsList } from "./CandDupCvsList";

interface IProps {
  candsListData: ICand[];
  candsSource: CandsSourceEnum;
}

export const CandsList = observer(({ candsListData, candsSource }: IProps) => {
  const { candsStore, positionsStore } = useStore();
  const [dupCv, setDupCv] = useState(0);
  let location = useLocation();
  const navigate = useNavigate();

  const listRef = useRef<any>(null);
  const [candsList, setCandsList] = useState<ICand[]>([]);

  useEffect(() => {
    if (candsListData) {
      setCandsList(candsListData?.slice(0, 50));
      listRef.current.scrollTop = 0;
    }
  }, [candsListData, setCandsList]); // eslint-disable-line react-hooks/exhaustive-deps

  const onScroll = useCallback(() => {
    const instance = listRef.current;

    if (
      instance.scrollHeight - instance.clientHeight <
      instance.scrollTop + 150
    ) {
      if (candsList) {
        const numRecords = candsList.length;

        const newPosList = candsList.concat(
          candsListData?.slice(numRecords, numRecords + 50)
        );

        setCandsList(newPosList);
      }

      console.log(instance.scrollTop);
    }
  }, [candsList, setCandsList]);

  useEffect(() => {
    const instance = listRef.current;

    instance.addEventListener("scroll", onScroll);

    return () => {
      instance.removeEventListener("scroll", onScroll);
    };
  }, [onScroll]);

  const handleDetachCand = (cand: ICand, index: number) => {
    if (candsStore.currentTabCandsLists === TabsCandsEnum.PositionCands) {
      candsStore.detachPosCand(cand, index);
    } else {
      candsStore.detachFolderCand(cand, index);
    }
  };

  return (
    <List
      ref={listRef}
      dense={true}
      sx={{
        backgroundColor: "#fff",
        height: "calc(100vh - 114px)",
        overflowY: "hidden",
        overflowX: "hidden",
        "&:hover ": {
          overflowY: "overlay",
        },
      }}
    >
      {candsList.map((cand, i) => {
        return (
          <ListItem
            key={cand.candidateId}
            dense
            disablePadding
            component="nav"
            sx={{
              flexDirection: "column",
              alignItems: "normal",
              pl: "10px",
            }}
          >
            <ListItemButton
              sx={{ pr: "4px", pl: "4px" }}
              selected={
                cand.candidateId === candsStore.candDisplay?.candidateId
              }
              onClick={() => {
                if (location.pathname !== "/cv") {
                  navigate(`/cv`);
                }
                candsStore.displayCvMain(cand);
              }}
            >
              <ListItemIcon
                onClick={(event) => {
                  event.stopPropagation();
                  event.preventDefault();

                  if (!dupCv || dupCv !== cand.cvId) {
                    setDupCv(cand.cvId);
                    candsStore.getDuplicatesCvsList(cand);
                  } else {
                    setDupCv(0);
                  }
                }}
                sx={{
                  visibility: !cand.hasDuplicates ? "hidden" : "visible",
                  minWidth: "45px",
                }}
              >
                <IconButton
                  color="primary"
                  aria-label="upload picture"
                  component="label"
                >
                  {dupCv && dupCv === cand.cvId ? (
                    <MdExpandLess />
                  ) : (
                    <MdExpandMore />
                  )}
                </IconButton>
              </ListItemIcon>

              {/* <ListItemText
                primary={format(new Date(cand.cvSent), "MMM d, yyyy")}
                sx={{
                  textAlign: "right",
                  color: "#bcc9d5",
                  fontSize: "0.775rem",
                  alignSelf: "start",
                  whiteSpace: "nowrap",
                }}
              /> */}

              <Box sx={{ width: "100%" }}>
                <Stack
                  direction="row-reverse"
                  sx={{
                    justifyContent: "space-between",
                  }}
                >
                  <ListItemText
                    primary={cand.emailSubject}
                    sx={{
                      overflow: "hidden",
                      whiteSpace: "nowrap",
                      textOverflow: "ellipsis",
                      textAlign: "right",
                      maxWidth: "21rem",
                      direction: "rtl",
                    }}
                  />
                  <ListItemText
                    primary={format(new Date(cand.cvSent), "MMM d, yyyy")}
                    sx={{ whiteSpace: "nowrap" }}
                  />
                </Stack>
                {cand.posStages?.length && (
                  <div style={{ fontSize: "0.775rem", paddingRight: "1rem" }}>
                    {candsStore.sortPosStage(cand.posStages).map((stage, i) => {
                      return (
                        <Stack
                          key={i}
                          direction="row-reverse"
                          gap={1}
                          title={candsStore.findStageName(stage.t)}
                          color={candsStore.findStageColor(stage.t)}
                          fontWeight={
                            candsSource === CandsSourceEnum.Position &&
                            stage.id === positionsStore.selectedPosition?.id
                              ? "bold"
                              : "none"
                          }
                        >
                          {" "}
                          <div>{format(new Date(stage.d), "dd/MM/yyyy")}</div>
                          <div>-</div>
                          <div
                            style={{
                              direction: "rtl",
                              overflow: "hidden",
                              whiteSpace: "nowrap",
                              textOverflow: "ellipsis",
                              paddingLeft: "1rem",
                              maxWidth: "21rem",
                              textDecoration:
                                candsSource === CandsSourceEnum.Position &&
                                stage.id === positionsStore.selectedPosition?.id
                                  ? "underline"
                                  : "none",
                            }}
                          >
                            {positionsStore.findPosName(stage.id)}
                          </div>
                        </Stack>
                        // <tr key={i}>
                        //   <td>{positionsStore.findPosName(stage.id)}</td>
                        //   <td>
                        //     {format(new Date(stage.d), "MMM dd, yyyy")}
                        //   </td>
                        // <td>{candsStore.findStageName(stage.t)}</td>
                        // </tr>
                      );
                    })}
                  </div>
                )}
              </Box>

              {candsStore.currentTabCandsLists !== TabsCandsEnum.AllCands &&
              candsStore.candDisplay?.candidateId === cand.candidateId ? (
                <ListItemIcon
                  onClick={(event) => {
                    event.stopPropagation();
                    handleDetachCand(cand, i);
                  }}
                >
                  <IconButton
                    sx={{
                      right: 0,
                      marginRight: 1,
                      color: "#d7d2d2",
                      "&:hover ": {
                        color: "#ffab55",
                      },
                    }}
                    color="primary"
                    aria-label="upload picture"
                    component="label"
                  >
                    <MdRemove />
                  </IconButton>
                </ListItemIcon>
              ) : (
                <div>&nbsp;&nbsp;</div>
              )}
            </ListItemButton>
            <Collapse in={cand.cvId === dupCv} timeout="auto" unmountOnExit>
              <CandDupCvsList />
            </Collapse>
          </ListItem>
        );
      })}
    </List>
  );
});
