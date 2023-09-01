import {
  Fab,
  List,
  ListItem,
  ListItemButton,
  ListItemText,
  Slide,
} from "@mui/material";
import { observer } from "mobx-react";
import { useCallback, useEffect, useRef, useState } from "react";
import { useStore } from "../../Hooks/useStore";
import { format } from "date-fns";
import styles from "./PositionsList.module.scss";
import { IPositionType } from "../../models/GeneralModels";
import classNames from "classnames";
import { isMobile } from "react-device-detect";
import { MdKeyboardArrowUp } from "react-icons/md";

export const PositionsTypesList = observer(() => {
  const { positionsStore } = useStore();

  const listRef = useRef<any>(null);
  const [posTypesList, setPosTypesList] = useState<IPositionType[]>([]);
  const [isScrolled, setIsScrolled] = useState(false);

  useEffect(() => {
    if (positionsStore.sortedPosTypesList) {
      setPosTypesList(positionsStore.sortedPosTypesList?.slice(0, 50));

      listRef.current.scrollTop = 0;
    }
  }, [positionsStore.sortedPosTypesList]); // eslint-disable-line react-hooks/exhaustive-deps

  const onScroll = useCallback(() => {
    const instance = listRef.current;
    const sTop = instance.scrollTop;

    if (sTop > 50) {
      setIsScrolled(true);
    } else {
      setIsScrolled(false);
    }

    if (instance.scrollHeight - instance.clientHeight < sTop + 150) {
      if (posTypesList) {
        const numRecords = posTypesList.length;
        const newPosList = posTypesList.concat(
          positionsStore.sortedPosTypesList?.slice(numRecords, numRecords + 50)
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
    <>
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
      <Slide direction="up" in={isScrolled} mountOnEnter unmountOnExit>
        <Fab
          size="medium"
          color="primary"
          sx={{
            zIndex: 999,
            position: "fixed",
            bottom: "7rem",
            left: "4rem",
            fontSize: "2rem",
          }}
          onClick={() => {
            listRef.current.scrollTop = 0;
          }}
        >
          <MdKeyboardArrowUp />
        </Fab>
      </Slide>
    </>
  );
});
