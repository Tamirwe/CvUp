import {
  Checkbox,
  List,
  ListItem,
  ListItemButton,
  ListItemText,
  Tooltip,
} from "@mui/material";
import { observer } from "mobx-react";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useStore } from "../../Hooks/useStore";
import { MdBookmarkBorder, MdBookmark } from "react-icons/md";

export const PositionsList = observer(() => {
  const { positionsStore } = useStore();
  const navigate = useNavigate();

  const [checked, setChecked] = useState(true);

  const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setChecked(event.target.checked);
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
      }}
    >
      {positionsStore.positionsList?.map((pos, i) => {
        return (
          <ListItem
            key={pos.id}
            dense
            disablePadding
            secondaryAction={
              <Tooltip title="Attach to position">
                <Checkbox
                  checked={checked}
                  onChange={handleChange}
                  sx={{ "& svg": { fontSize: 22 } }}
                  icon={<MdBookmarkBorder />}
                  checkedIcon={<MdBookmark />}
                />
              </Tooltip>
            }
          >
            <ListItemButton
              selected={pos.id === positionsStore.currentPosition.id}
              onClick={() => {
                navigate(`/position/${pos.id}`);

                // positionsStore.GetPosition(pos.id);
              }}
            >
              <ListItemText primary={pos.name} sx={{ pr: 5 }} />
              <ListItemText
                primary={pos.updated.toString()}
                sx={{
                  textAlign: "right",
                  color: "#bcc9d5",
                  fontSize: "0.775rem",
                }}
              />
            </ListItemButton>
          </ListItem>
        );
      })}
    </List>
  );
});
