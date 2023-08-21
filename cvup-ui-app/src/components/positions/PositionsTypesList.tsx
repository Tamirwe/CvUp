import { List, ListItem, ListItemButton, ListItemText } from "@mui/material";
import { observer } from "mobx-react";
import { useCallback, useEffect, useRef, useState } from "react";
import { useStore } from "../../Hooks/useStore";
import { format } from "date-fns";
import styles from "./PositionsList.module.scss";
import { IPositionType } from "../../models/GeneralModels";
import classNames from "classnames";
import { isMobile } from "react-device-detect";

export const PositionsTypesList = observer(() => {
  const { positionsStore } = useStore();

  const listRef = useRef<any>(null);
  const [posTypesList, setPosTypesList] = useState<IPositionType[]>([]);

  useEffect(() => {
    if (positionsStore.positionsTypesList) {
      setPosTypesList(positionsStore.positionsTypesList?.slice(0, 50));

      listRef.current.scrollTop = 0;
    }
  }, [positionsStore.positionsTypesList]); // eslint-disable-line react-hooks/exhaustive-deps

  const onScroll = useCallback(() => {
    const instance = listRef.current;

    if (
      instance.scrollHeight - instance.clientHeight <
      instance.scrollTop + 150
    ) {
      if (posTypesList) {
        const numRecords = posTypesList.length;
        const newPosList = posTypesList.concat(
          positionsStore.positionsTypesList?.slice(numRecords, numRecords + 50)
        );
        setPosTypesList(newPosList);
      }
    }
  }, [posTypesList]);

  useEffect(() => {
    const instance = listRef.current;

    instance.addEventListener("scroll", onScroll);

    return () => {
      instance.removeEventListener("scroll", onScroll);
    };
  }, [onScroll]);

  return (
    <List
      ref={listRef}
      className={classNames({
        [styles.posList]: true,
        [styles.isMobile]: isMobile,
      })}
    >
      {posTypesList.map((pos, i) => {
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
              selected={pos.id === positionsStore.selectedPositionType?.id}
              onClick={() => positionsStore.positionTypeClick(pos.id)}
            >
              <div
                className={classNames({
                  [styles.listItemDate]: true,
                  [styles.isMobile]: isMobile,
                })}
              >
                {pos.dateUpdated &&
                  format(new Date(pos.dateUpdated), "MMM d, yyyy")}
              </div>
              <ListItemText
                primary={pos.typeName}
                sx={{
                  textAlign: "right",
                  paddingRight: 2,
                }}
              />
            </ListItemButton>
          </ListItem>
        );
      })}
    </List>
  );
});
