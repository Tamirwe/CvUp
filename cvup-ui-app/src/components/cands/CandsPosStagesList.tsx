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
}

export const CandsPosStagesList = observer(({ candsSource, cand }: IProps) => {
  const { candsStore, positionsStore } = useStore();

  return (
    <div
      style={{ direction: "ltr", fontSize: "0.775rem", paddingRight: "0.2rem" }}
    >
      {cand.posStages &&
        candsStore.sortPosStage(cand.posStages).map((stage, i) => {
          const posNameCompany = positionsStore.findPosName(stage.id);
          const isCurrentPosition =
            candsSource === CandsSourceEnum.Position &&
            positionsStore.selectedPosition?.id === stage.id;

          if (!posNameCompany) {
            return;
          }

          return (
            <div
              key={i}
              className={classNames({
                [styles.listItemPosStages]: true,
                [styles.listItemPosStagesCurrent]: isCurrentPosition,
              })}
            >
              {cand.candidateId === candsStore.candDisplay?.candidateId && (
                <IconButton
                  size="small"
                  sx={{ ml: 1 }}
                  onClick={(event) => {
                    event.stopPropagation();
                    event.preventDefault();
                    candsStore.detachPosCand(cand, stage.id, i);
                  }}
                >
                  <MdRemove />
                </IconButton>
              )}{" "}
              <div
                className={classNames({
                  [styles.listItem]: true,
                  [styles.listItemSelected]:
                    cand.candidateId === candsStore.candDisplay?.candidateId,
                })}
                style={{
                  color: candsStore.findStageColor(stage.t),
                }}
                title={`Status: ${candsStore.findStageName(stage.t)}`}
                {...(cand.candidateId ===
                  candsStore.candDisplay?.candidateId && {
                  onClick: async (event) => {
                    event.stopPropagation();
                    event.preventDefault();

                    const posId = stage.id;
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
                  {format(new Date(stage.d), "MMM d, yyyy")}
                </div>
                <div>&nbsp;-&nbsp;</div>
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

                <div>
                  <MdOutlineMarkEmailUnread />
                </div>
              </div>
            </div>
          );
        })}
    </div>
  );
});
