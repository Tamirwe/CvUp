import { IconButton } from "@mui/material";
import { format } from "date-fns";
import { observer } from "mobx-react-lite";
import { MdOutlineMarkEmailUnread, MdRemove } from "react-icons/md";
import { useStore } from "../../Hooks/useStore";
import { CandsSourceEnum } from "../../models/GeneralEnums";
import { ICand } from "../../models/GeneralModels";
import styles from "./CandsList.module.scss";
import classNames from "classnames";
import { isMobile } from "react-device-detect";

interface IProps {
  candsSource: CandsSourceEnum;
  cand: ICand;
  isCanNavigate?: boolean;
}

export const CandsPosStagesList = observer(
  ({ candsSource, cand, isCanNavigate = true }: IProps) => {
    const { candsStore, positionsStore } = useStore();

    return (
      <div
        style={{
          direction: "ltr",
          fontSize: "0.775rem",
          paddingRight: "0.2rem",
        }}
      >
        {cand.posStages &&
          candsStore.sortPosStage(cand.posStages).map((stage, i) => {
            const posNameCompany = positionsStore.findPosName(stage._pid);

            if (!posNameCompany) {
              return;
            }

            return (
              <div
                key={i}
                className={classNames({
                  [styles.listItemPosStages]: true,
                  // [styles.listItemPosStagesCurrent]: isCurrentPosition,
                })}
              >
                <div
                  className={classNames({
                    [styles.listItem]: true,
                    [styles.listItemSelected]:
                      cand.candidateId === candsStore.candDisplay?.candidateId,
                  })}
                  style={{
                    color: candsStore.findStageColor(stage._tp),
                    textAlign: "right",
                  }}
                  title={` ${candsStore.findStageName(stage._tp)} - ${format(
                    new Date(stage._dt),
                    "MMM d, yyyy"
                  )}`}
                  {...(cand.candidateId ===
                    candsStore.candDisplay?.candidateId &&
                    isCanNavigate && {
                      onClick: async (event) => {
                        event.stopPropagation();
                        event.preventDefault();

                        const posId = stage._pid;
                        await positionsStore.positionClick(posId, true);
                        positionsStore.setRelatedPositionToCandDisplay();
                        candsStore.setDisplayCandOntopPCList();
                      },
                    })}
                >
                  <div
                    className={classNames({
                      [styles.listItemDate]: true,
                      [styles.isMobile]: isMobile,
                    })}
                  >
                    {format(new Date(stage._dt), "MMM d, yyyy")}
                  </div>
                  <div
                    style={{
                      direction: "rtl",
                      overflow: "hidden",
                      whiteSpace: "nowrap",
                      textOverflow: "ellipsis",
                      paddingLeft: "1rem",
                    }}
                  >
                    {posNameCompany}
                  </div>
                </div>
              </div>
            );
          })}
      </div>
    );
  }
);
