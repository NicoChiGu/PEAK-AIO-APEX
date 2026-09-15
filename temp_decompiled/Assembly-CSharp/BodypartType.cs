using System;

// Token: 0x02000036 RID: 54
public enum BodypartType
{
	// Token: 0x04000339 RID: 825
	Hip,
	// Token: 0x0400033A RID: 826
	Mid,
	// Token: 0x0400033B RID: 827
	Torso,
	// Token: 0x0400033C RID: 828
	Neck,
	// Token: 0x0400033D RID: 829
	Head,
	// Token: 0x0400033E RID: 830
	Arm_L,
	// Token: 0x0400033F RID: 831
	Elbow_L,
	// Token: 0x04000340 RID: 832
	Hand_L,
	// Token: 0x04000341 RID: 833
	Arm_R,
	// Token: 0x04000342 RID: 834
	Elbow_R,
	// Token: 0x04000343 RID: 835
	Hand_R,
	// Token: 0x04000344 RID: 836
	Leg_L,
	// Token: 0x04000345 RID: 837
	Knee_L,
	// Token: 0x04000346 RID: 838
	Foot_L,
	// Token: 0x04000347 RID: 839
	Leg_R,
	// Token: 0x04000348 RID: 840
	Knee_R,
	// Token: 0x04000349 RID: 841
	Foot_R,
	// Token: 0x0400034A RID: 842
	Item,
	// Token: 0x0400034B RID: 843
	Mouth,
	// Token: 0x0400034C RID: 844
	Jaw_U,
	// Token: 0x0400034D RID: 845
	Jaw_D,
	// Token: 0x0400034E RID: 846
	Jaw_L,
	// Token: 0x0400034F RID: 847
	Jaw_R,
	// Token: 0x04000350 RID: 848
	Hip_L,
	// Token: 0x04000351 RID: 849
	Hip_R,
	// Token: 0x04000352 RID: 850
	Shoulder_L,
	// Token: 0x04000353 RID: 851
	Shoulder_R,
	// Token: 0x04000354 RID: 852
	Toe_L,
	// Token: 0x04000355 RID: 853
	Toe_R,
	// Token: 0x04000356 RID: 854
	Finger_L,
	// Token: 0x04000357 RID: 855
	Finger_R,
	// Token: 0x04000358 RID: 856
	Unnasigned_1,
	// Token: 0x04000359 RID: 857
	Unnasigned_2,
	// Token: 0x0400035A RID: 858
	Unnasigned_3,
	// Token: 0x0400035B RID: 859
	Unnasigned_4,
	// Token: 0x0400035C RID: 860
	Unnasigned_5,
	// Token: 0x0400035D RID: 861
	Unnasigned_6,
	// Token: 0x0400035E RID: 862
	Unnasigned_7,
	// Token: 0x0400035F RID: 863
	Unnasigned_8,
	// Token: 0x04000360 RID: 864
	Tail_1,
	// Token: 0x04000361 RID: 865
	Tail_2,
	// Token: 0x04000362 RID: 866
	Tail_3,
	// Token: 0x04000363 RID: 867
	Tail_4,
	// Token: 0x04000364 RID: 868
	Tail_5,
	// Token: 0x04000365 RID: 869
	Tail_6,
	// Token: 0x04000366 RID: 870
	Tail_7,
	// Token: 0x04000367 RID: 871
	Tail_8,
	// Token: 0x04000368 RID: 872
	Extra_1,
	// Token: 0x04000369 RID: 873
	Extra_2,
	// Token: 0x0400036A RID: 874
	Extra_3,
	// Token: 0x0400036B RID: 875
	Extra_4,
	// Token: 0x0400036C RID: 876
	Extra_5,
	// Token: 0x0400036D RID: 877
	Extra_6,
	// Token: 0x0400036E RID: 878
	Extra_7,
	// Token: 0x0400036F RID: 879
	Extra_8,
	// Token: 0x04000370 RID: 880
	Extra_9,
	// Token: 0x04000371 RID: 881
	Extra_10,
	// Token: 0x04000372 RID: 882
	Extra_11,
	// Token: 0x04000373 RID: 883
	Extra_12,
	// Token: 0x04000374 RID: 884
	Leg_1_L,
	// Token: 0x04000375 RID: 885
	Knee_1_L,
	// Token: 0x04000376 RID: 886
	Foot_1_L,
	// Token: 0x04000377 RID: 887
	Leg_1_R,
	// Token: 0x04000378 RID: 888
	Knee_1_R,
	// Token: 0x04000379 RID: 889
	Foot_1_R,
	// Token: 0x0400037A RID: 890
	Leg_2_L,
	// Token: 0x0400037B RID: 891
	Knee_2_L,
	// Token: 0x0400037C RID: 892
	Foot_2_L,
	// Token: 0x0400037D RID: 893
	Leg_2_R,
	// Token: 0x0400037E RID: 894
	Knee_2_R,
	// Token: 0x0400037F RID: 895
	Foot_2_R,
	// Token: 0x04000380 RID: 896
	Leg_3_L,
	// Token: 0x04000381 RID: 897
	Knee_3_L,
	// Token: 0x04000382 RID: 898
	Foot_3_L,
	// Token: 0x04000383 RID: 899
	Leg_3_R,
	// Token: 0x04000384 RID: 900
	Knee_3_R,
	// Token: 0x04000385 RID: 901
	Foot_3_R,
	// Token: 0x04000386 RID: 902
	Leg_4_L,
	// Token: 0x04000387 RID: 903
	Knee_4_L,
	// Token: 0x04000388 RID: 904
	Foot_4_L,
	// Token: 0x04000389 RID: 905
	Leg_4_R,
	// Token: 0x0400038A RID: 906
	Knee_4_R,
	// Token: 0x0400038B RID: 907
	Foot_4_R,
	// Token: 0x0400038C RID: 908
	Leg_5_L,
	// Token: 0x0400038D RID: 909
	Knee_5_L,
	// Token: 0x0400038E RID: 910
	Foot_5_L,
	// Token: 0x0400038F RID: 911
	Leg_5_R,
	// Token: 0x04000390 RID: 912
	Knee_5_R,
	// Token: 0x04000391 RID: 913
	Foot_5_R,
	// Token: 0x04000392 RID: 914
	Leg_6_L,
	// Token: 0x04000393 RID: 915
	Knee_6_L,
	// Token: 0x04000394 RID: 916
	Foot_6_L,
	// Token: 0x04000395 RID: 917
	Leg_6_R,
	// Token: 0x04000396 RID: 918
	Knee_6_R,
	// Token: 0x04000397 RID: 919
	Foot_6_R,
	// Token: 0x04000398 RID: 920
	Leg_7_L,
	// Token: 0x04000399 RID: 921
	Knee_7_L,
	// Token: 0x0400039A RID: 922
	Foot_7_L,
	// Token: 0x0400039B RID: 923
	Leg_7_R,
	// Token: 0x0400039C RID: 924
	Knee_7_R,
	// Token: 0x0400039D RID: 925
	Foot_7_R,
	// Token: 0x0400039E RID: 926
	Leg_8_L,
	// Token: 0x0400039F RID: 927
	Knee_8_L,
	// Token: 0x040003A0 RID: 928
	Foot_8_L,
	// Token: 0x040003A1 RID: 929
	Leg_8_R,
	// Token: 0x040003A2 RID: 930
	Knee_8_R,
	// Token: 0x040003A3 RID: 931
	Foot_8_R,
	// Token: 0x040003A4 RID: 932
	Leg_9_L,
	// Token: 0x040003A5 RID: 933
	Knee_9_L,
	// Token: 0x040003A6 RID: 934
	Foot_9_L,
	// Token: 0x040003A7 RID: 935
	Leg_9_R,
	// Token: 0x040003A8 RID: 936
	Knee_9_R,
	// Token: 0x040003A9 RID: 937
	Foot_9_R,
	// Token: 0x040003AA RID: 938
	Leg_10_L,
	// Token: 0x040003AB RID: 939
	Knee_10_L,
	// Token: 0x040003AC RID: 940
	Foot_10_L,
	// Token: 0x040003AD RID: 941
	Leg_10_R,
	// Token: 0x040003AE RID: 942
	Knee_10_R,
	// Token: 0x040003AF RID: 943
	Foot_10_R,
	// Token: 0x040003B0 RID: 944
	Spine_1,
	// Token: 0x040003B1 RID: 945
	Spine_2,
	// Token: 0x040003B2 RID: 946
	Spine_3,
	// Token: 0x040003B3 RID: 947
	Spine_4,
	// Token: 0x040003B4 RID: 948
	Spine_5,
	// Token: 0x040003B5 RID: 949
	Spine_6,
	// Token: 0x040003B6 RID: 950
	Spine_7,
	// Token: 0x040003B7 RID: 951
	Spine_8,
	// Token: 0x040003B8 RID: 952
	Spine_9,
	// Token: 0x040003B9 RID: 953
	Spine_10,
	// Token: 0x040003BA RID: 954
	Jiggle_1_L,
	// Token: 0x040003BB RID: 955
	Jiggle_1_R,
	// Token: 0x040003BC RID: 956
	Jiggle_2_L,
	// Token: 0x040003BD RID: 957
	Jiggle_2_R,
	// Token: 0x040003BE RID: 958
	Jiggle_3_L,
	// Token: 0x040003BF RID: 959
	Jiggle_3_R,
	// Token: 0x040003C0 RID: 960
	Jiggle_4_L,
	// Token: 0x040003C1 RID: 961
	Jiggle_4_R,
	// Token: 0x040003C2 RID: 962
	Jiggle_5_L,
	// Token: 0x040003C3 RID: 963
	Jiggle_5_R,
	// Token: 0x040003C4 RID: 964
	Jiggle_6_L,
	// Token: 0x040003C5 RID: 965
	Jiggle_6_R,
	// Token: 0x040003C6 RID: 966
	Jiggle_7_L,
	// Token: 0x040003C7 RID: 967
	Jiggle_7_R,
	// Token: 0x040003C8 RID: 968
	Jiggle_8_L,
	// Token: 0x040003C9 RID: 969
	Jiggle_8_R,
	// Token: 0x040003CA RID: 970
	Jiggle_9_L,
	// Token: 0x040003CB RID: 971
	Jiggle_9_R,
	// Token: 0x040003CC RID: 972
	Jiggle_10_L,
	// Token: 0x040003CD RID: 973
	Jiggle_10_R,
	// Token: 0x040003CE RID: 974
	Finger_1_1_R,
	// Token: 0x040003CF RID: 975
	Finger_1_2_R,
	// Token: 0x040003D0 RID: 976
	Finger_1_3_R,
	// Token: 0x040003D1 RID: 977
	Finger_2_1_R,
	// Token: 0x040003D2 RID: 978
	Finger_2_2_R,
	// Token: 0x040003D3 RID: 979
	Finger_2_3_R,
	// Token: 0x040003D4 RID: 980
	Finger_3_1_R,
	// Token: 0x040003D5 RID: 981
	Finger_3_2_R,
	// Token: 0x040003D6 RID: 982
	Finger_3_3_R,
	// Token: 0x040003D7 RID: 983
	Finger_4_1_R,
	// Token: 0x040003D8 RID: 984
	Finger_4_2_R,
	// Token: 0x040003D9 RID: 985
	Finger_4_3_R,
	// Token: 0x040003DA RID: 986
	Finger_5_1_R,
	// Token: 0x040003DB RID: 987
	Finger_5_2_R,
	// Token: 0x040003DC RID: 988
	Finger_5_3_R,
	// Token: 0x040003DD RID: 989
	Finger_1_1_L,
	// Token: 0x040003DE RID: 990
	Finger_1_2_L,
	// Token: 0x040003DF RID: 991
	Finger_1_3_L,
	// Token: 0x040003E0 RID: 992
	Finger_2_1_L,
	// Token: 0x040003E1 RID: 993
	Finger_2_2_L,
	// Token: 0x040003E2 RID: 994
	Finger_2_3_L,
	// Token: 0x040003E3 RID: 995
	Finger_3_1_L,
	// Token: 0x040003E4 RID: 996
	Finger_3_2_L,
	// Token: 0x040003E5 RID: 997
	Finger_3_3_L,
	// Token: 0x040003E6 RID: 998
	Finger_4_1_L,
	// Token: 0x040003E7 RID: 999
	Finger_4_2_L,
	// Token: 0x040003E8 RID: 1000
	Finger_4_3_L,
	// Token: 0x040003E9 RID: 1001
	Finger_5_1_L,
	// Token: 0x040003EA RID: 1002
	Finger_5_2_L,
	// Token: 0x040003EB RID: 1003
	Finger_5_3_L,
	// Token: 0x040003EC RID: 1004
	Final
}
