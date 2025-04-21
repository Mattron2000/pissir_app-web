package group14.pissir.util;

public class TOPICS {
	private static final String IOT = "iot";

	public class SENSOR {
		private static final String BASE = IOT + "/sensors";

		/**
		 * Get the topic 'iot/sensors/{id}/status'
		 *
		 * @param id
		 * @return the topic for the status of the sensor
		 */
		public static String getStatus(String id) {
			return SENSOR.BASE + "/" + id + "/status";
		}

		/**
		 * Get the topic 'iot/sensors/{id}/check'
		 *
		 * @param id
		 * @return the topic for the check of the sensor
		 */
		public static String getCheck(String id) {
			return SENSOR.BASE + "/" + id + "/check";
		}

		/**
		 * Get the topic 'iot/sensors/{id}/switch'
		 *
		 * @param id
		 * @return the topic for the switch of the sensor
		 */
		public static String getSwitch(String id) {
			return SENSOR.BASE + "/" + id + "/switch";
		}
	}

	public class MONITOR {
		private static final String BASE = IOT + "/monitor";

		public static String getCheck() {
			return MONITOR.BASE + "/check";
		}

        public static String getCount() {
			return MONITOR.BASE + "/count";
        }
	}

	public class MOBILE {
		private static final String BASE = IOT + "/mobiles";

		public static String getNotify(String phoneNumber) {
			return MOBILE.BASE + "/" + phoneNumber + "/notify";
		}

        public static String getData(String phoneNumber) {
			return MOBILE.BASE + "/" + phoneNumber + "/data";
        }

        public static String getClose(String phoneNumber) {
			return MOBILE.BASE + "/" + phoneNumber + "/close";
        }

        public static String getNew(String phone_number) {
			return MOBILE.BASE + "/" + phone_number + "/new";
        }
	}

	public class MWBOT {
		private static final String BASE = IOT + "/mwbot";

		public static String getCheck() {
			return MWBOT.BASE + "/check";
        }

		public static String getNewRequest() {
			return MWBOT.BASE + "/new_request";
		}

        public static String getDebug() {
			return MWBOT.BASE + "/debug";
        }

		public static String getAck() {
			return MWBOT.BASE + "/ack";
        }
	}

	private static final String LWT = "LWT";

	public static String getLwt() {
		return LWT;
	}
}
