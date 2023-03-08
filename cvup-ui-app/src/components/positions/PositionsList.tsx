import {
  Checkbox,
  List,
  ListItem,
  ListItemButton,
  ListItemText,
} from "@mui/material";
import { observer } from "mobx-react";
import { useEffect } from "react";
import { useStore } from "../../Hooks/useStore";
import { MdPersonAddAlt1 } from "react-icons/md";
import { format } from "date-fns";
import styles from "./PositionsList.module.scss";
import { TabsCandsEnum } from "../../models/GeneralEnums";

export const PositionsList = observer(() => {
  const { positionsStore, candsStore } = useStore();

  const handleAttachPosCandCv = (posId: number) => {
    candsStore.attachPosCandCv(posId);
  };

  // const handleDetachPosCand = (
  //   positionId: number,
  //   cand: ICand,
  //   index: number
  // ) => {
  //   candsStore.detachPosCandidate(positionId, cand, index);
  // };

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
              selected={pos.id === positionsStore.selectedPosition?.id}
              onClick={() => {
                positionsStore.setPosSelected(pos.id);
                candsStore.getPositionCands(pos.id);
                candsStore.currentTabCandsLists = TabsCandsEnum.PositionCands;
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
              <Checkbox
                checked={
                  candsStore.candDisplay && candsStore.candDisplay.candPosIds
                    ? candsStore.candDisplay.candPosIds.indexOf(pos.id) > -1
                    : false
                }
                disabled={
                  candsStore.candDisplay &&
                  candsStore.candDisplay.candPosIds &&
                  candsStore.candDisplay.candPosIds.indexOf(pos.id) > -1
                }
                onChange={(event) => {
                  event.stopPropagation();
                  event.preventDefault();
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
                icon={<MdPersonAddAlt1 />}
                checkedIcon={<MdPersonAddAlt1 />}
              />
            </ListItemButton>
          </ListItem>
        );
      })}
    </List>
  );
});
