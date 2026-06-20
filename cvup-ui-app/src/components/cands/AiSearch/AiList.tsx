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
                    id={`cand-${cand.candidateId}`}
                    style={{
                      whiteSpace: "normal",
                    }}
                  >
                    <div>
                      <span style={{ color: "#312f2f", fontSize: "0.625rem" }}>
                        {cand.score}
                      </span>
                      <span style={{ color: "#a38c14", fontWeight: "bold" }}>
                        {`${cand.nameAI ? ` - ${cand.nameAI}` : ` - ${cand.firstName} ${cand.lastName}`}`}
                      </span>
                      <span style={{ color: "#9b9b9b" }}>
                        {cand.estimateAgeAI != null && cand.estimateAgeAI > 0 && ` - ${cand.estimateAgeAI}`}
                      </span>
                      <span style={{ color: "#9b9b9b" }}>
                        {cand.locationAI && ` - ${cand.locationAI}`}
                      </span>
                    </div>
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
                        color: "#045d93",
                      }}
                    >
                      <table
                        style={{
                          width: "fitContent",
                          borderSpacing: "10px 4px",
                        }}
                      >
                        {cand.jobsTitlesAI.map((jobTitle, i) => {
                          return (
                            <tr>
                              <td>
                                <span style={{ color: "#007edb" }}>
                                  {cand.companiesAI && cand.companiesAI[i]}{" "}
                                </span>
                              </td>
                              <td>{jobTitle}</td>
                            </tr>
                          );
                        })}
                      </table>
                    </div>

                    {/* <div
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
                    </div> */}
                  </div>

                  {/* <div
                    style={{
                      whiteSpace: "normal",
                      fontSize: "0.875rem",
                      color: "#51a798",
                      paddingTop: "4px",
                    }}
                  >
                    {cand.educationAI}
                  </div> */}
                </div>
              </Box>
            </ListItemButton>
          </ListItem>
        );
      })}
    </List>
  );
});
