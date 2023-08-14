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
import { MdExpandLess, MdExpandMore } from "react-icons/md";
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
    const [dupOpenCandId, setDupOpenCandId] = useState(0);
    const [listCands, setListCands] = useState<ICand[]>([]);

    useEffect(() => {
      if (candsListData) {
        setListCands([
          ...candsListData?.slice(0, Math.max(listCands.length, 50)),
        ]);
      }

      setDupOpenCandId(0);
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
          height: advancedOpen ? "calc(100vh - 156px)" : "calc(100vh - 107px)",
        }}
        className={classNames({
          [styles.candList]: true,
          [styles.isMobile]: isMobile,
        })}
      >
        {listCands.map((cand, i) => {
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
                // "& .MuiButtonBase-root": {
                //   ml: "2px",
                // },
              }}
            >
              <ListItemButton
                sx={{ pl: 0 }}
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
                      title="Email subject"
                      style={{
                        color: cand.isSeen ? "unset" : "green",
                      }}
                      className={classNames({
                        [styles.listItemText]: true,
                        [styles.isMobile]: isMobile,
                      })}
                    >
                      {/* {cand.score} */}
                      {candsSource === CandsSourceEnum.AllCands
                        ? cand.emailSubject
                        : `${cand.firstName || ""} ${cand.lastName || ""}`}
                    </div>

                    <div
                      style={{
                        // width: "87%",
                        display: "flex",
                        // justifyContent: "space-between",
                        alignItems: "center",
                      }}
                    >
                      <div
                        title="Cv sent date"
                        className={classNames({
                          [styles.listItemDate]: true,
                          [styles.isMobile]: isMobile,
                        })}
                      >
                        {format(new Date(cand.cvSent), "MMM d, yy")}
                      </div>
                      <ListItemIcon
                        sx={{
                          visibility: !cand.hasDuplicates
                            ? "hidden"
                            : "visible",
                        }}
                        onClick={async (event) => {
                          event.stopPropagation();
                          event.preventDefault();

                          // if (!isMobile) {
                          if (location.pathname !== "/cv") {
                            navigate(`/cv`);
                          }

                          await candsStore.displayCv(cand, candsSource);
                          await candsStore.getDuplicatesCvsList(cand);

                          if (dupOpenCandId !== cand.candidateId) {
                            setDupOpenCandId(cand.candidateId);
                          } else {
                            setDupOpenCandId(0);
                          }

                          // if (candsStore.duplicateCvsCandId !== cand.candidateId) {
                          //   candsStore.duplicateCvsCandId = cand.candidateId;
                          //   candsStore.getDuplicatesCvsList(cand);
                          // } else {
                          //   candsStore.duplicateCvsCandId = 0;
                          // }
                          //}
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
                      </ListItemIcon>
                    </div>
                  </div>
                  {cand.posStages && cand.posStages?.length > 0 && (
                    <CandsPosStagesList cand={cand} candsSource={candsSource} />
                  )}
                </Box>
              </ListItemButton>
              {dupOpenCandId > 0 && (
                <Collapse
                  in={dupOpenCandId === cand.candidateId}
                  timeout="auto"
                  unmountOnExit
                >
                  <CandDupCvsList candPosCvId={cand.posCvId} />
                </Collapse>
              )}
            </ListItem>
          );
        })}
      </List>
    );
  }
);
