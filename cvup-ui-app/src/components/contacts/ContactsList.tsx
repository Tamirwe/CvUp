import { List } from "@mui/material";
import { observer } from "mobx-react-lite";
import { useStore } from "../../Hooks/useStore";

export const ContactsList = observer(() => {
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
    ></List>
  );
});
