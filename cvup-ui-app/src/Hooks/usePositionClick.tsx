import { useStore } from "./useStore";
import { TabsCandsEnum } from "../models/GeneralEnums";

export const usePositionClick = () => {
  const { positionsStore, candsStore, generalStore } = useStore();

  const handlePositionClick = (posId: number, candId?: number) => {
    if (
      candsStore.currentTabCandsLists !== TabsCandsEnum.PositionCands ||
      positionsStore.selectedPosition?.id !== posId
    ) {
      positionsStore.setPosSelected(posId, candId);
      candsStore.getPositionCands();
      candsStore.currentTabCandsLists = TabsCandsEnum.PositionCands;
    }
  };
  return handlePositionClick;
};
