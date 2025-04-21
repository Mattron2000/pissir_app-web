package group14.pissir.util;

import com.google.gson.Gson;

public class JsonWrapper {
	private static Gson gson = new Gson();

	public static String toJson(Object obj) {
		return JsonWrapper.gson.toJson(obj);
	}

    public static <T> T fromJson(String message, Class<T> classData) {
		return JsonWrapper.gson.fromJson(message, classData);
    }
}
