import {
  Checkbox,
  Collapse,
  IconButton,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Tooltip,
} from "@mui/material";
import { observer } from "mobx-react";
import { useEffect, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { useStore } from "../../Hooks/useStore";
import { MdRemove, MdPostAdd } from "react-icons/md";
import { format } from "date-fns";
import styles from "./PositionsList.module.scss";
import { ICand } from "../../models/GeneralModels";

export const PositionsList = observer(() => {
  let location = useLocation();
  const { positionsStore, cvsStore } = useStore();
  const navigate = useNavigate();

  const handleAttachPosCandCv = (posId: number) => {
    cvsStore.attachPosCandCv(posId);
  };

  const handleDetachPosCand = (
    positionId: number,
    cand: ICand,
    index: number
  ) => {
    cvsStore.detachPosCandidate(positionId, cand, index);
  };

  useEffect(() => {
    (async () => {
      await positionsStore.getPositionsList(false);
    })();
  }, []); // eslint-disable-line react-hooks/exhaustive-deps

  return (
    <List
      sx={{
        overflowY: "auto",
        width: "98%",
        "&:hover": {},
      }}
    >
      {positionsStore.positionsList?.map((pos, i) => {
        return (
          <ListItem
            key={pos.id}
            dense
            disablePadding
            className={styles.showAddCv}
            sx={{
              "& .MuiListItemSecondaryAction-root": { right: 0 },
              flexDirection: "column",
              alignItems: "normal",
              pl: "18px",
            }}
          >
            <ListItemButton
              sx={{ pl: "4px" }}
              selected={pos.id === positionsStore.posSelected?.id}
              onClick={() => {
                // navigate(`/position/${pos.id}`);
                positionsStore.setPosSelected(pos.id);
                cvsStore.getPositionCands(pos.id);
              }}
            >
              <ListItemText
                primary={format(new Date(pos.updated), "MMM d, yyyy")}
                sx={{
                  textAlign: "right",
                  alignSelf: "start",
                  color: "#bcc9d5",
                  fontSize: "0.775rem",
                }}
              />
              <ListItemText primary={pos.name} />
              <Tooltip title="Attach to position">
                <Checkbox
                  checked={
                    cvsStore.candDisplaying &&
                    cvsStore.candDisplaying.candPosIds
                      ? cvsStore.candDisplaying.candPosIds.indexOf(pos.id) > -1
                      : false
                  }
                  onChange={(event) => {
                    event.stopPropagation();
                    handleAttachPosCandCv(pos.id);
                  }}
                  onClick={(event) => {
                    event.stopPropagation();
                  }}
                  sx={{
                    right: 0,
                    marginRight: "2px",
                    color: "#d7d2d2",
                    "& svg": { fontSize: 22 },
                    "&.Mui-checked": {
                      color: "#ff8d00",
                    },
                  }}
                  icon={<MdPostAdd />}
                  checkedIcon={<MdPostAdd />}
                />
              </Tooltip>
            </ListItemButton>

            <Collapse
              in={pos.id === positionsStore.posSelected?.id}
              timeout="auto"
              unmountOnExit
            >
              <List
                component="div"
                disablePadding
                dense={true}
                sx={{
                  backgroundColor: "#fbfbfb",
                  border: "1px solid #ffdcdc",
                  maxHeight: "300px",
                  overflowY: "hidden",
                  "&:hover ": {
                    overflow: "overlay",
                  },
                }}
              >
                {cvsStore.posCandsList.map((cand, i) => {
                  return (
                    <ListItemButton
                      key={`${cand.cvId}dup`}
                      sx={{ fontSize: "0.75rem", pl: 1 }}
                      selected={cand.cvId === cvsStore.candDisplaying?.cvId}
                      onClick={() => {
                        if (location.pathname !== "/cv") {
                          navigate(`/cv`);
                        }
                        cvsStore.displayCvPosition(cand);
                      }}
                    >
                      <ListItemText
                        primary={format(new Date(cand.cvSent), "MMM d, yyyy")}
                        sx={{
                          textAlign: "right",
                          color: "#bcc9d5",
                          fontSize: "0.775rem",
                          alignSelf: "start",
                          "& span": { fontSize: "0.75rem" },
                        }}
                      />
                      <ListItemText
                        sx={{ "& span, p": { fontSize: "0.75rem" } }}
                        primary={cand.candidateName}
                        secondary={cand.emailSubject}
                      />
                      <ListItemIcon
                        onClick={(event) => {
                          event.stopPropagation();
                          handleDetachPosCand(pos.id, cand, i);
                        }}
                      >
                        <Tooltip title="Detach Cv">
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
                        </Tooltip>
                      </ListItemIcon>
                    </ListItemButton>
                  );
                })}
              </List>
            </Collapse>
          </ListItem>
        );
      })}
    </List>
  );
});
