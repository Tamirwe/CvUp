import { observer } from "mobx-react";
import { useStore } from "../../../Hooks/useStore";
import { isMobile } from "react-device-detect";
import classNames from "classnames";
import { Box, List, ListItem, ListItemButton } from "@mui/material";
import { useRef } from "react";
import styles from "./AiList.module.scss";
import { useLocation, useNavigate } from "react-router";
import { ICand } from "../../../models/GeneralModels";
import { CandsSourceEnum } from "../../../models/GeneralEnums";

interface IProps {
  candsListData: ICand[];
  candsSource: CandsSourceEnum;
}

export const AiList = observer(({ candsListData, candsSource }: IProps) => {
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
      {candsStore.aiCandsResults.map((cand, i) => {
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
                await candsStore.displayCv(cand, candsSource);

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
                  <div
                    style={{
                      whiteSpace: "normal",
                      fontWeight: 600,
                    }}
                  >
                    {cand.score} - {cand.nameAI} - {cand.currentTitleAI} -{" "}
                    <span style={{ color: "#a38c14", fontWeight: "bold" }}>
                      {cand.locationAI}
                    </span>
                    <div
                      style={{
                        color: "#68a6e9",
                        fontWeight: 600,
                      }}
                    >
                      {cand.professionWordsAI &&
                        cand.professionWordsAI.join(", ")}
                      {"."}
                    </div>
                    <div
                      style={{
                        color: "#68a6e9",
                        fontWeight: 600,
                      }}
                    >
                      {cand.professionSkillsAI &&
                        cand.professionSkillsAI.join(", ")}
                      {"."}
                    </div>
                  </div>
                  <div
                    style={{
                      whiteSpace: "normal",
                      fontSize: "0.875rem",
                      color: "#03458d",
                      paddingTop: "4px",
                      fontWeight: 600,
                    }}
                  >
                    {cand.companiesAI}
                  </div>
                  {/* <div
                    style={{
                      whiteSpace: "normal",
                      fontSize: "0.875rem",
                      color: "#68a6e9",
                      paddingTop: "4px",
                      fontWeight: 600,
                    }}
                  >
                    {cand.professionAI}
                  </div> */}
                  <div
                    style={{
                      whiteSpace: "normal",
                      color: "gray",
                      paddingTop: "4px",
                    }}
                  >
                    {cand.summaryAI}
                  </div>
                  <div
                    style={{
                      whiteSpace: "normal",
                      fontSize: "0.875rem",
                      color: "#51a798",
                      paddingTop: "4px",
                      fontWeight: 600,
                    }}
                  >
                    {cand.educationAI}
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
