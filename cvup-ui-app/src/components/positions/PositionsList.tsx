import { List, ListItem, ListItemButton, ListItemText } from "@mui/material";
import { observer } from "mobx-react";
import { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { useStore } from "../../Hooks/useStore";

export const PositionsList = observer(() => {
  const { positionsStore } = useStore();
  const navigate = useNavigate();

  useEffect(() => {
    (async () => {
      positionsStore.getPositionsList(false);
    })();
  }, []); // eslint-disable-line react-hooks/exhaustive-deps

  return (
    <List dense={true} sx={{ maxWidth: "20rem" }}>
      {positionsStore.positionsList?.map((pos, i) => {
        return (
          <ListItem key={pos.id}>
            <ListItemButton
              onClick={() => {
                navigate(`/position/${pos.id}`);

                // positionsStore.GetPosition(pos.id);
              }}
            >
              <ListItemText primary={pos.name} />
            </ListItemButton>
          </ListItem>
        );
      })}
    </List>
  );
});
