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

export const PositionsList = observer(() => {
  let location = useLocation();
  const { positionsStore, cvsStore } = useStore();
  const navigate = useNavigate();

  const handleAttachPocCv = (
    event: React.ChangeEvent<HTMLInputElement>,
    posId: number
  ) => {
    if (event.target.checked) {
      cvsStore.attachPosCandCv(posId);
    }
  };

  const handleDetachPosCv = (
    posId: number,
    cvId: number,
    candidateId: number
  ) => {
    cvsStore.detachPosCandidate(posId, cvId, candidateId);
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
                cvsStore.getPositionCvs(pos.id);
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
                    cvsStore.cvDisplayed && cvsStore.cvDisplayed.candPosIds
                      ? cvsStore.cvDisplayed.candPosIds.indexOf(pos.id) > -1
                      : false
                  }
                  onChange={(event) => {
                    event.stopPropagation();
                    handleAttachPocCv(event, pos.id);
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
                {cvsStore.posCvsList.map((cv, i) => {
                  return (
                    <ListItemButton
                      key={`${cv.cvId}dup`}
                      sx={{ fontSize: "0.75rem", pl: 1 }}
                      selected={cv.cvId === cvsStore.cvDisplayed?.cvId}
                      onClick={() => {
                        if (location.pathname !== "/cv") {
                          navigate(`/cv`);
                        }
                        cvsStore.displayCvPosition(cv);
                      }}
                    >
                      <ListItemText
                        primary={format(new Date(cv.cvSent), "MMM d, yyyy")}
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
                        primary={cv.candidateName}
                        secondary={cv.emailSubject}
                      />
                      <ListItemIcon
                        onClick={(event) => {
                          event.stopPropagation();
                          handleDetachPosCv(pos.id, cv.cvId, cv.candidateId);
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
                      {/* <Tooltip title="Attach to position">
                        <Checkbox
                          checked={
                            cvsStore.cvDisplayed &&
                            cvsStore.cvDisplayed.candPosIds
                              ? cvsStore.cvDisplayed.candPosIds.indexOf(
                                  pos.id
                                ) > -1
                              : false
                          }
                          onChange={(event) => {
                            event.stopPropagation();
                            handleDetachPosCv(event, pos.id);
                          }}
                          onClick={(event) => {
                            event.stopPropagation();
                          }}
                          sx={{
                            right: 0,
                            marginRight: 1,
                            color: "#d7d2d2",
                            "& svg": { fontSize: 22 },
                            "&.Mui-checked": {
                              color: "#aba6a0",
                            },
                          }}
                          icon={<MdRemove />}
                          checkedIcon={<MdRemove />}
                        />
                      </Tooltip> */}
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
