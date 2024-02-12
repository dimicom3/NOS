/*
 * Copyright (c) 2018 STMicroelectronics
 *
 * SPDX-License-Identifier: Apache-2.0
 */

#include <zephyr/kernel.h>
#include <zephyr/device.h>
#include <zephyr/drivers/sensor.h>
#include <stdio.h>
#include <zephyr/sys/util.h>
#include <zephyr/drivers/gpio.h>

#define LED0_NODE DT_ALIAS(led0)

static const struct gpio_dt_spec led = GPIO_DT_SPEC_GET(LED0_NODE, gpios);

int main(void)
{
	struct sensor_value temp, hum;

	const struct device *const hts221 = DEVICE_DT_GET_ONE(st_hts221);
	int ret;

	if (!device_is_ready(hts221)) {
		printk("%s: device not ready.\n", hts221->name);
		return 0;
	}

	if (!device_is_ready(led.port)) {
		printf("led errorr\n");
		return;
	}

	ret = gpio_pin_configure_dt(&led, GPIO_OUTPUT_ACTIVE);

	if (ret < 0) {
		printf("led errorr\n");
		return;
	}


	while (1) {

		if (sensor_sample_fetch(hts221) < 0) {
			printf("HTS221 Sensor sample update error\n");
			return 0;
		}


		sensor_channel_get(hts221, SENSOR_CHAN_AMBIENT_TEMP, &temp);
		sensor_channel_get(hts221, SENSOR_CHAN_HUMIDITY, &hum);

		double temp_val = sensor_value_to_double(&temp);

		printf("HTS221: Temperature: %.1f C\n", temp_val);

		printf("HTS221: Relative Humidity: %.1f%%\n",
		       sensor_value_to_double(&hum));

		if(temp_val > 23){
			ret = gpio_pin_set_dt(&led, 1);
			if (ret < 0) {
				printf("led errorr\n");			
				return;
			}else
			{
				printf("LED ON");
			}
		}else{
			ret = gpio_pin_set_dt(&led, 0);
			if (ret < 0) {
				printf("led errorr\n");			
				return;
			}else
			{
				printf("LED OFF");
			}
		}


		k_sleep(K_MSEC(2000));
	}
}
