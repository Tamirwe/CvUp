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
        backgroundColor: "#fff",
        height: "calc(100vh - 96px)",
        overflowY: "scroll",
        overflowX: "hidden",
      }}
    >
      {listCands.map((cand, i) => {
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
              onClick={(event) => {
                event.stopPropagation();
                event.preventDefault();
                setDupCv(0);

                if (location.pathname !== "/cv") {
                  navigate(`/cv`);
                }
                candsStore.displayCvMain(cand);
              }}
            >
              <ListItemIcon
                itemID="dupListIcon"
                onClick={(event) => {
                  event.stopPropagation();
                  event.preventDefault();

                  if (location.pathname !== "/cv") {
                    navigate(`/cv`);
                  }
                  candsStore.displayCvMain(cand);

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
                    primary={
                      candsSource === CandsSourceEnum.Position
                        ? format(new Date(cand.dateAttached), "MMM d, yyyy")
                        : format(new Date(cand.cvSent), "MMM d, yyyy")
                    }
                    sx={{ whiteSpace: "nowrap" }}
                  />
                </Stack>
                {cand.posStages?.length && (
                  <CandsPosStagesList cand={cand} candsSource={candsSource} />
                )}
              </Box>
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
