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
  Typography,
} from "@mui/material";
import { format } from "date-fns";
import { observer } from "mobx-react-lite";
import { useEffect } from "react";
import { MdExpandLess, MdExpandMore } from "react-icons/md";
import { useLocation, useNavigate } from "react-router-dom";
import { useStore } from "../../Hooks/useStore";

export const CvsList = observer(() => {
  const { cvsStore } = useStore();
  let location = useLocation();
  const navigate = useNavigate();

  useEffect(() => {
    cvsStore.getCvsList();
  }, []); // eslint-disable-line react-hooks/exhaustive-deps

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
      {cvsStore.cvsList.map((cv, i) => {
        return (
          <ListItem
            key={cv.cvId}
            dense
            disablePadding
            component="nav"
            sx={{
              flexDirection: "column",
              alignItems: "normal",
              pl: "18px",
            }}
          >
            <ListItemButton
              selected={cv.cvId === cvsStore.cvSelected?.cvId}
              onClick={() => {
                if (location.pathname !== "/cv") {
                  navigate(`/cv`);
                }
                cvsStore.getCv(cv);
                cvsStore.GetDuplicatesCvsList(cv);
              }}
            >
              <ListItemIcon>
                <Tooltip title="Attach to position">
                  <IconButton
                    color="primary"
                    aria-label="upload picture"
                    component="label"
                  >
                    <MdExpandMore />
                  </IconButton>
                </Tooltip>
              </ListItemIcon>

              <ListItemText
                primary={format(new Date(cv.cvSent), "MMM d, yyyy")}
                sx={{
                  textAlign: "right",
                  color: "#bcc9d5",
                  fontSize: "0.775rem",
                  alignSelf: "start",
                }}
              />
              <ListItemText
                primary={cv.candidateName}
                secondary={cv.emailSubject}
              />
            </ListItemButton>
            <Collapse
              in={cv.cvId === cvsStore.cvSelected?.cvId}
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
                {cvsStore.duplicatesCvsList.map((cv, i) => {
                  return (
                    <ListItemButton
                      key={`${cv.cvId}dup`}
                      sx={{ fontSize: "0.75rem", pl: 4 }}
                      selected={cv.cvId === cvsStore.cvDuplicateSelected?.cvId}
                      onClick={() => {
                        if (location.pathname !== "/cv") {
                          navigate(`/cv`);
                        }
                        cvsStore.getCvDuplicate(cv);
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
