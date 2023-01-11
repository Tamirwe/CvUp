import { List, ListItem, ListItemButton, ListItemText } from "@mui/material";
import { observer } from "mobx-react-lite";
import { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { useStore } from "../../Hooks/useStore";

export const CvsList = observer(() => {
  const { cvsStore } = useStore();
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
        overflowY: "auto",
      }}
    >
      {cvsStore.cvsList.map((cv, i) => {
        return (
          <ListItem key={cv.cvId}>
            <ListItemButton
              onClick={() => {
                // navigate(`/cv/${escape(cv.keyId)}`);
                cvsStore.getCv(cv);
              }}
            >
              <ListItemText primary={cv.emailSubject} />
            </ListItemButton>
          </ListItem>
        );
      })}
    </List>
  );
});
