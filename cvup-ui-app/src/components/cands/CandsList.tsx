import {
  Collapse,
  IconButton,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Tooltip,
} from "@mui/material";
import { format } from "date-fns";
import { observer } from "mobx-react-lite";
import { useState } from "react";
import { MdExpandLess, MdExpandMore, MdRemove } from "react-icons/md";
import { useLocation, useNavigate } from "react-router-dom";
import { useStore } from "../../Hooks/useStore";
import { ICand } from "../../models/GeneralModels";
import { CandDupCvsList } from "./CandDupCvsList";

interface IProps {
  candsList: ICand[];
}

export const CandsList = observer(({ candsList }: IProps) => {
  const { cvsStore } = useStore();
  const [dupCv, setDupCv] = useState(0);
  let location = useLocation();
  const navigate = useNavigate();

  const handleDetachPosCand = (cand: ICand, index: number) => {
    cvsStore.detachPosCandidate(cand, index);
  };

  return (
    <List
      dense={true}
      sx={{
        backgroundColor: "#fff",
        height: "calc(100vh - 81px)",
        overflowY: "hidden",
        "&:hover ": {
          overflow: "overlay",
        },
      }}
    >
      {candsList.map((cand, i) => {
        return (
          <ListItem
            key={cand.cvId}
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
              selected={cand.candidateId === cvsStore.candDisplay?.candidateId}
              onClick={() => {
                if (location.pathname !== "/cv") {
                  navigate(`/cv`);
                }
                cvsStore.displayCvMain(cand);
              }}
            >
              <ListItemIcon
                onClick={(event) => {
                  event.stopPropagation();
                  event.preventDefault();

                  if (!dupCv || dupCv !== cand.cvId) {
                    setDupCv(cand.cvId);
                    cvsStore.getDuplicatesCvsList(cand);
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

              <ListItemText
                primary={format(new Date(cand.cvSent), "MMM d, yyyy")}
                sx={{
                  textAlign: "right",
                  color: "#bcc9d5",
                  fontSize: "0.775rem",
                  alignSelf: "start",
                  whiteSpace: "nowrap",
                }}
              />
              <ListItemText
                primary={cand.candidateName}
                secondary={cand.emailSubject}
              />
              {cvsStore.currentTabCandsList === "positionCandsList" ? (
                <ListItemIcon
                  onClick={(event) => {
                    event.stopPropagation();
                    handleDetachPosCand(cand, i);
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