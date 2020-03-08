
# Latham: Create Long-Running Timelapse Videos

![.NET Core](https://github.com/abock/latham/workflows/.NET%20Core/badge.svg)


Latham is a fully featured multi-source video stream recording, indexing, and
timelapsing project geared towards creating long-running timelapse footage.

* **Record**: from multiple sources (e.g. RTSP camera feeds) on a schedule
(expressed in crontab notation):
  > _Save 5 seconds of video from each camera every ten minutes._

* **Index**: keep an index of all recordings up to date for fast querying when
  it comes time to compose a timelapse. An index can be rebuilt from a path
  pattern and kept up to date directly from the _record_ operation as well.

* **Filter**: filter the index for recorded clips matching time-based predicates
  > _Select all recordings for the `back` camera, Monday through Friday, between
    8am and 5pm, and not during the last week of February, 2020_.

* **Compose**: create a timelapse video from filtered clips.
  > _Given all footage that makes it through the filter, create a timelapse
    lasting one minute._

All of the above can be expressed in a Latham project file:

```json
{
  "name": "Latham Way Construction Project",
  "description": "2020 Home Rebuild",
  "recordings": {
    "outputPath": "{tag}/{DateTime.Now:yyyy/MM/dd}/{tag}-{DateTime.Now:yyyy-MM-dd_HH.mm.sszzz}.mp4",
    "duration": "5s",
    "schedule": "*/10 * * * *",
    "unifiProtectEndpoints": [
      "https://user:password@unifi-cloud-key-host:7443"
    ],
    "sources": [
      {
        "tag": "back",
        "uri": "rtsp://unifi-cloud-key-host:7447/camera-channel-id1"
      },
      {
        "tag": "front",
        "uri": "rtsp://unifi-cloud-key-host:7447/camera-channel-id2"
      },
      {
        "tag": "driveway",
        "uri": "rtsp://unifi-cloud-key-host:7447/camera-channel-id3"
      }
    ]
  },
  "ingestions": [
    {
      "basePath": "/Volumes/LathamViewData",
      "pathGlob": "**/*.mp4",
      "pathFilter": "[\/\\\\](?<tag>\\w+)-(?<yyyy>\\d{4})-(?<MM>\\d{2})-(?<dd>\\d{2})_(?<HH>\\d{2}).(?<mm>\\d{2}).(?<ss>\\d{2})(?<z>[+-]\\d{4})?\\.mp4$"
    }
  ],
  "timelapses": [
    {
      "tagMatch": ".*",
      "include": [
        "Monday .. Friday",
        "8am .. 5pm"
      ],
      "exclude": [
        "2020-2-2",
        "2020-2-5 .. 2020-2-9",
        "2020-2-26 .. 2020-2-27"
      ]
    }
  ]
}
```