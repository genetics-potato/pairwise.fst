library(LDheatmap)
data("CEUData")

MyHeatmap <- LDheatmap(CEUSNP, CEUDist, LDmeasure = "r",
 title = "Pairwise LD in r^2", add.map = TRUE,
 SNP.name = c("rs2283092", "rs6979287"), color = grey.colors(20),
 name = "myLDgrob", add.key = TRUE)