import { List, ListItemButton, ListItemText } from "@mui/material";
import { format } from "date-fns";
import { observer } from "mobx-react-lite";
import { useLocation, useNavigate } from "react-router-dom";
import { useStore } from "../../Hooks/useStore";
import { CvDisplayedListEnum } from "../../models/GeneralEnums";

export const CandDupCvsList = observer(() => {
  const { candsStore } = useStore();
  let location = useLocation();
  const navigate = useNavigate();

  return (
    <List
      component="div"
      disablePadding
      dense={true}
      sx={{
        backgroundColor: "#fbfbfb",
        border: "1px solid #ffdcdc",
        maxHeight: "300px",
        overflowY: "hidden",
        fontSize: "0.75rem",
        "&:hover ": {
          overflow: "overlay",
        },
      }}
    >
      {candsStore.candDupCvsList &&
        candsStore.candDupCvsList.map((dupCv, i) => {
          return (
            <ListItemButton
              key={`${dupCv.cvId}dup`}
              sx={{ fontSize: "0.75rem", pl: 4, textAlign: "right" }}
              selected={dupCv.cvId === candsStore.candDisplay?.cvId}
              onClick={() => {
                if (location.pathname !== "/cv") {
                  navigate(`/cv`);
                }
                candsStore.displayCvDuplicate(
                  dupCv,
                  CvDisplayedListEnum.CandsList
                );
              }}
            >
              <ListItemText
                sx={{
                  "& span, p": { fontSize: "0.75rem", textAlign: "right" },
                }}
                primary={dupCv.emailSubject}
                // secondary={dupCv.emailSubject}
              />
              <ListItemText
                primary={format(new Date(dupCv.cvSent), "MMM d, yyyy")}
                sx={{
                  maxWidth: "5rem",
                  "& span": { fontSize: "0.75rem" },
                }}
              />
            </ListItemButton>
          );
        })}
    </List>
  );
});
