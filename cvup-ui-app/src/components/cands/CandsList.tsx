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
    const { candsStore, generalStore } = useStore();
    let location = useLocation();
    const navigate = useNavigate();

    const listRef = useRef<any>(null);
    const [dupOpenCandId, setDupOpenCandId] = useState(0);
    const [listCands, setListCands] = useState<ICand[]>([]);

    useEffect(() => {
      if (candsListData) {
        setListCands([...candsListData?.slice(0, 50)]);
        listRef.current.scrollTop = 0;
      }
    }, [candsListData, setListCands]); // eslint-disable-line react-hooks/exhaustive-deps

    const onScroll = useCallback(() => {
      const instance = listRef.current;

      if (
        instance.scrollHeight - instance.clientHeight <
        instance.scrollTop + 150
      ) {
        if (listCands) {
          const numRecords = listCands.length;

          const newPosList = listCands.concat(
            candsListData?.slice(numRecords, numRecords + 50)
          );

          setListCands(newPosList);
        }

        console.log(instance.scrollTop);
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
                "& .MuiButtonBase-root": {
                  pl: "1px",
                },
              }}
            >
              <ListItemButton
                sx={{ pl: 0 }}
                selected={
                  cand.candidateId === candsStore.candDisplay?.candidateId
                }
                onClick={(event) => {
                  event.stopPropagation();
                  event.preventDefault();

                  if (isMobile) {
                    generalStore.rightDrawerOpen = false;
                  }

                  if (location.pathname !== "/cv") {
                    navigate(`/cv`);
                  }

                  candsStore.displayCv(cand, candsSource);
                }}
              >
                <Box
                  sx={{
                    width: "100%",
                  }}
                >
                  <div style={{ display: "flex" }}>
                    <div
                      style={{
                        width: "87%",
                        display: "flex",
                        justifyContent: "space-between",
                        alignItems: "center",
                      }}
                    >
                      <div
                        className={classNames({
                          [styles.listItemText]: true,
                          [styles.isMobile]: isMobile,
                        })}
                      >
                        {cand.emailSubject}
                      </div>
                      <div
                        className={classNames({
                          [styles.listItemDate]: true,
                          [styles.isMobile]: isMobile,
                        })}
                      >
                        {candsSource === CandsSourceEnum.Position
                          ? format(new Date(cand.dateAttached), "MMM d, yyyy")
                          : format(new Date(cand.cvSent), "MMM d, yyyy")}
                      </div>
                    </div>

                    <ListItemIcon
                      sx={{
                        visibility: !cand.hasDuplicates ? "hidden" : "visible",
                      }}
                      onClick={(event) => {
                        event.stopPropagation();
                        event.preventDefault();

                        // if (!isMobile) {
                        if (location.pathname !== "/cv") {
                          navigate(`/cv`);
                        }

                        candsStore.displayCv(cand, candsSource);
                        candsStore.getDuplicatesCvsList(cand);

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
                  {cand.posStages?.length && (
                    <CandsPosStagesList cand={cand} candsSource={candsSource} />
                  )}
                </Box>
              </ListItemButton>
              <Collapse
                in={dupOpenCandId === cand.candidateId}
                timeout="auto"
                unmountOnExit
              >
                <CandDupCvsList />
              </Collapse>
            </ListItem>
          );
        })}
      </List>
    );
  }
);
