import { useStore } from "./useStore";
import { TabsCandsEnum } from "../models/GeneralEnums";
import { isMobile } from "react-device-detect";

export const usePositionClick = () => {
  const { positionsStore, candsStore, generalStore } = useStore();

  const handlePositionClick = (posId: number, candId?: number) => {
    if (
      candsStore.currentTabCandsLists !== TabsCandsEnum.PositionCands ||
      positionsStore.selectedPosition?.id !== posId
    ) {
      positionsStore.setPosSelected(posId, candId);
      candsStore.getPositionCands();

      if (isMobile) {
        generalStore.leftDrawerOpen = false;
        generalStore.rightDrawerOpen = true;
      }

      candsStore.currentTabCandsLists = TabsCandsEnum.PositionCands;
    }
  };
  return handlePositionClick;
};