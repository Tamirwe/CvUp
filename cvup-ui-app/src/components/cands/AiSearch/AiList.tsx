import { observer } from "mobx-react";
import { useStore } from "../../../Hooks/useStore";
import { isMobile } from "react-device-detect";
import classNames from "classnames";
import { Box, List, ListItem, ListItemButton } from "@mui/material";
import { useRef } from "react";
import styles from "./AiList.module.scss";
import { useLocation, useNavigate } from "react-router";

export const AiList = observer(() => {
  const { candsStore, generalStore } = useStore();
  const listRef = useRef<any>(null);
  const navigate = useNavigate();
  let location = useLocation();

  return (
    <List
      ref={listRef}
      dense={true}
      sx={{ height: isMobile ? "70vh" : "calc(100vh - 125px)" }}
      className={classNames({
        [styles.candList]: true,
        [styles.isMobile]: isMobile,
      })}
    >
      {candsStore.aiSearchResults.map((cand, i) => {
        return (
          <ListItem
            key={`${cand.candidateId}${i}`}
            dense
            disablePadding
            component="nav"
            sx={{
              flexDirection: "column",
              alignItems: "normal",
              direction: "rtl",
              pl: "2px",
            }}
          >
            <ListItemButton
              sx={{ pl: 0 }}
              selected={
                cand.candidateId === candsStore.candDisplay?.candidateId
              }
              onClick={async (event) => {
                event.stopPropagation();
                event.preventDefault();

                if (location.pathname !== "/cv") {
                  navigate(`/cv`);
                }
                // await candsStore.displayCv(cand, candsSource);

                if (isMobile) {
                  generalStore.rightDrawerOpen = false;
                }
              }}
            >
              <Box sx={{ width: "100%" }}>
                <div
                  className={classNames({
                    [styles.listItemText]: true,
                    [styles.isMobile]: isMobile,
                  })}
                >
                  {cand.name} - {cand.currentTitle} -{" "}
                  <span style={{ color: "#a38c14", fontWeight: "bold" }}>
                    {cand.location}
                  </span>
                  <div
                    style={{
                      whiteSpace: "normal",
                      fontSize: "0.875rem",
                      color: "#03458d",
                      paddingTop: "4px",
                      fontWeight: 600,
                    }}
                  >
                    {cand.companies}
                  </div>
                  <div
                    style={{
                      whiteSpace: "normal",
                      color: "gray",
                      paddingTop: "4px",
                    }}
                  >
                    {cand.summary}
                  </div>
                </div>
              </Box>
            </ListItemButton>
          </ListItem>
        );
      })}
    </List>
  );
});
