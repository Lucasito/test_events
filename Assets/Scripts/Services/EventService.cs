using System.Collections;
using System.Collections.Generic;
using System.Text;
using Data;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Services {
    public class EventService : MonoBehaviour {
        public const string LEVEL_START = "levelStart";
        public const string GET_REWARD = "getReward";
        public const string SPEND_COINS = "spendCoins";

        private const string SAVE_EVENT_TYPE_PREF = "save_event_type";
        private const string SAVE_EVENT_DATA_PREF = "save_event_data";

        [SerializeField] private string _serverUrl;
        [SerializeField] private float _cooldownBeforeSend;

        private EventListData _eventList;
        private float _nextSendTime;
        private bool _sendingProcess;
        private bool _existEvents;

        private void Awake() {
            _eventList = new EventListData();
            _eventList.events = new List<EventData>();
            _sendingProcess = false;
            _existEvents = false;
            LoadEvents();
        }

        public void TrackEvent(string type, string data) {
            AddEvent(new EventData(type, data));
            CheckSendEvents();
        }

        public void Update() {
            CheckSendEvents();
        }

        private void CheckSendEvents() {
            if (Time.time >= _nextSendTime && !_sendingProcess && _existEvents) {
                StartCoroutine(Post(JsonConvert.SerializeObject(_eventList)));
            }
        }

        IEnumerator Post(string json) {
            _sendingProcess = true;
            Debug.Log("[EventService] Send: " + json);
            using (var request = new UnityWebRequest(_serverUrl, "POST")) {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                yield return request.SendWebRequest();

                Debug.Log("[EventService] Status Code: " + request.responseCode);
                if (request.responseCode == 200) {
                    ClearEvents();
                }

                _nextSendTime = Time.time + _cooldownBeforeSend;
                _sendingProcess = false;
            }
        }

        private void ClearEvents() {
            for (int i = 1; i < _eventList.events.Count; i++) {
                PlayerPrefs.DeleteKey(SAVE_EVENT_TYPE_PREF + i);
                PlayerPrefs.DeleteKey(SAVE_EVENT_DATA_PREF + i);
            }

            PlayerPrefs.Save();
            _eventList.events.Clear();
            _existEvents = false;
        }

        private void AddEvent(EventData eventData) {
            _eventList.events.Add(eventData);
            PlayerPrefs.SetString(SAVE_EVENT_TYPE_PREF + _eventList.events.Count, eventData.type);
            PlayerPrefs.SetString(SAVE_EVENT_DATA_PREF + _eventList.events.Count, eventData.data);
            PlayerPrefs.Save();
            _existEvents = true;
        }

        private void LoadEvents() {
            int i = 1;
            if (!PlayerPrefs.HasKey(SAVE_EVENT_TYPE_PREF + i)) {
                _existEvents = false;
                return;
            }

            while (PlayerPrefs.HasKey(SAVE_EVENT_TYPE_PREF + i)) {
                _eventList.events.Add(new EventData(PlayerPrefs.GetString(SAVE_EVENT_TYPE_PREF + i),
                    PlayerPrefs.GetString(SAVE_EVENT_DATA_PREF + i)));
                i++;
            }

            _existEvents = true;
        }
    }
}