import { List, ListItemButton, ListItemText } from "@mui/material";
import { format } from "date-fns";
import { observer } from "mobx-react-lite";
import { useLocation, useNavigate } from "react-router-dom";
import { useStore } from "../../Hooks/useStore";

export const CandDupCvsList = observer(() => {
  const { cvsStore } = useStore();
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
        "&:hover ": {
          overflow: "overlay",
        },
      }}
    >
      {cvsStore.candDupCvsList.map((dupCv, i) => {
        return (
          <ListItemButton
            key={`${dupCv.cvId}dup`}
            sx={{ fontSize: "0.75rem", pl: 4 }}
            selected={dupCv.cvId === cvsStore.candDisplay?.cvId}
            onClick={() => {
              if (location.pathname !== "/cv") {
                navigate(`/cv`);
              }
              cvsStore.displayCvDuplicate(dupCv);
            }}
          >
            <ListItemText
              primary={format(new Date(dupCv.cvSent), "MMM d, yyyy")}
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
              primary={dupCv.candidateName}
              secondary={dupCv.emailSubject}
            />
          </ListItemButton>
        );
      })}
    </List>
  );
});
