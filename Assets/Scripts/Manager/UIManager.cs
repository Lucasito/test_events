using Services;
using UnityEngine;

namespace Manager {
    public class UIManager : MonoBehaviour {
        [SerializeField] private EventService _eventService;

        public void LevelStartButtonClick() {
            _eventService.TrackEvent(EventService.LEVEL_START, "1");
        }

        public void GetRewardButtonClick() {
            _eventService.TrackEvent(EventService.GET_REWARD, "10");
        }

        public void SpendCoinsButtonClick() {
            _eventService.TrackEvent(EventService.SPEND_COINS, "5");
        }
    }
}