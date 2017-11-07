import matplotlib
import matplotlib.pyplot as plt
import numpy as np
import pandas as pd
import sklearn
from sklearn import linear_model

def prepare_country_stats(oecd_bli, gdp_per_capita):
	full_country_stats = pd.merge(left=oecd_bli, right=gdp_per_capita, left_index=True, right_index=True)
	full_country_stats.sort_values(by="GDP per capita", inplace=True)
	full_country_stats
	return full_country_stats
	
#load the data
oecd_bli = pd.read_csv("oecd_bli_2016.csv", thousands=',')
oecd_bli = oecd_bli[oecd_bli["INEQUALITY"]=="TOT"]
oecd_bli = oecd_bli.pivot(index="Country", columns="Indicator", values="Value")
gdp_per_capita = pd.read_csv("gdp_per_capita.csv", thousands=',', delimiter='\t',
				encoding='latin1', na_values="n/a")
gdp_per_capita.rename(columns={"2015": "GDP per capita"}, inplace=True)
gdp_per_capita.set_index("Country", inplace=True)
gdp_per_capita.head(2)

#prepare the data
country_stats = prepare_country_stats(oecd_bli, gdp_per_capita)
x = np.c_[country_stats["GDP per capita"]]
y = np.c_[country_stats["Life satisfaction"]]

#visualize the data
country_stats.plot(kind='scatter', x='GDP per capita', y='Life satisfaction')


#select a linear model
prediction_model = sklearn.linear_model.LinearRegression()

#train the model
prediction_model.fit(x,y)
t0, t1 = prediction_model.intercept_[0], prediction_model.coef_[0][0]
plt.plot(x, t0 + t1*x, "b")


#make a prediction for Cyprus
X_new = [[22587]] #Cypru's GDP per capita
Y_linear = prediction_model.predict(X_new)

#use k_nearest neighor regression
prediction_model = sklearn.neighbors.KNeighborsRegressor(n_neighbors=3)

#train the model
prediction_model.fit(x,y)

#make a prediction for Cyprus
Y_neighbor = prediction_model.predict(X_new)

plt.plot(X_new, Y_linear, "gs")
plt.plot(X_new, Y_neighbor, "rs")

plt.annotate('Linear regression', xy=(X_new[0][0], Y_linear[0][0]), xytext=(X_new[0][0]-10000, Y_linear[0][0]+1), arrowprops = dict(facecolor='black', width=0.5, shrink=0.1, headwidth=5))
plt.annotate('K-neighbor regression', xy=(X_new[0][0], Y_neighbor[0][0]), xytext=(X_new[0][0], Y_neighbor[0][0]-0.8), arrowprops = dict(facecolor='black', width=0.5, shrink=0.1, headwidth=5))
plt.show()