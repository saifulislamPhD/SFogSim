﻿var app = angular.module('sfog', ['ngMaterial']);
app.directive('myDirective', function () {
	return {
		restrict: 'E',
		templateUrl: 'Home/index.cshtml',
		css: 'Content/Site.css'
	}
});
app.controller('SFogController', ['$scope', 'fog', 'cloud', 'edge', 'createDS', 'CreateEdgeDS', '$http', '$window', function ($scope, fog, cloud, edge, createDS, CreateEdgeDS, $http, $window) {
	var vm = this;
	// #region reset

	vm.resetFog = function () {
		vm.FogMaster = {};
		vm.FogServer = angular.copy(vm.FogMaster);
	};
	vm.resetEdge = function () {
		vm.FogMaster = {};
		vm.EdgeServer = angular.copy(vm.FogMaster);
	};
	vm.resetCloud = function () {
		vm.CloudMaster = {};
		vm.CloudServer = angular.copy(vm.CloudMaster);
	};

	vm.resetTuple = function () {
		vm.TupleMaster = {};
		vm.Tuple = angular.copy(vm.TupleMaster);
	};
	vm.resetETuple = function () {
		vm.TupleMaster = {};
		vm.ETuple = angular.copy(vm.TupleMaster);
	};
	// #endregion

	// #region methods POST calls
	vm.RunFogSimulationwithData = function (fogdata, tupledata, policyType, GatewayPolicyType, NodelevelPolicyTypes, CloudService, CloufServiceForFog, selectedDcForFog, Gateway, Cooperation, FogType) {
		fog.SimulateFogPost(fogdata, tupledata, policyType, GatewayPolicyType, NodelevelPolicyTypes, CloudService, CloufServiceForFog, selectedDcForFog, Gateway, Cooperation, FogType, function (output) {
			vm.downloadFogFile();
		});
	};
	vm.RunEdgeSimulation = function (edgedata, tupledata, policyType, CloudService, CloufServiceForFog, selectedDcForFog, Gateway, Cooperation, FogType) {
		console.log("I am clicked");
		edge.SimulateEdge(edgedata, tupledata, policyType, CloudService, CloufServiceForFog, selectedDcForFog, Gateway, Cooperation, FogType, function (output) {
			vm.downloadEdgeFile();
		});
	}
	vm.RunFogCreateData = function (fogdata, tupledata, policyType, CloudService, CloufServiceForFog, selectedDcForFog, Gateway, Cooperation, FogType) {
		$('#floatBarsG').show();
		createDS.GenerateFogData(fogdata, tupledata, policyType, CloudService, CloufServiceForFog, selectedDcForFog, Gateway, Cooperation, FogType, function (output) {
			$('#floatBarsG').hide();
		});
	};
	vm.RunEdgeCreateData = function (edgedata, tupledata, policyType, CloudService, CloufServiceForFog, selectedDcForFog, Gateway, Cooperation, FogType) {
		$('#EfloatBarsG').show();
		CreateEdgeDS.GenerateEdgeData(edgedata, tupledata, policyType, CloudService, CloufServiceForFog, selectedDcForFog, Gateway, Cooperation, FogType, function (output) {
			$('#EfloatBarsG').hide();
		});
	};
	vm.RunCloudSimulationwithData = function (tupledata, FCFS, Service, DataCenter) {
		cloud.SimulateCloudPost(tupledata, FCFS, Service, DataCenter, function (output) {
			vm.downloadCloudFile();
		});
	};
	// #endregion
	// #region file download
	vm.downloadFogFile = function () {
		console.log("I am called")
		// Use an arraybuffer
		$http.get("/api/main/GetFogFile", { responseType: 'arraybuffer' })
			.success(function (data, status, headers) {
				var octetStreamMime = 'application/octet-stream';
				var success = false;

				headers = headers();

				var filename = "Results_Fog.xlsx";

				var contentType = headers['content-type'] || octetStreamMime;

				try {
					var blob = new Blob([data], { type: contentType });
					if (navigator.msSaveBlob)
						navigator.msSaveBlob(blob, filename);
					else {
						var saveBlob = navigator.webkitSaveBlob || navigator.mozSaveBlob || navigator.saveBlob;
						if (saveBlob === undefined) throw "Not supported";
						saveBlob(blob, filename);
					}

					success = true;
				} catch (ex) {
				}

				if (!success) {
					var urlCreator = window.URL || window.webkitURL || window.mozURL || window.msURL;
					if (urlCreator) {
						var link = document.createElement('a');
						if ('download' in link) {
							try {
								var blob = new Blob([data], { type: contentType });
								var url = urlCreator.createObjectURL(blob);
								link.setAttribute('href', url);

								link.setAttribute("download", filename);

								var event = document.createEvent('MouseEvents');
								event.initMouseEvent('click', true, true, window, 1, 0, 0, 0, 0, false, false, false, false, 0, null);
								link.dispatchEvent(event);

								success = true;
							} catch (ex) {
							}
						}

						if (!success) {
							try {
								var blob = new Blob([data], { type: octetStreamMime });
								var url = urlCreator.createObjectURL(blob);
								window.location = url;

								success = true;
							} catch (ex) {
							}
						}
					}
				}

				if (!success) {
					window.open(httpPath, '_blank', '');
				}
			})
			.error(function (data, status) {
				$scope.errorDetails = "Request failed with status: " + status;
			});
	}
	vm.downloadEdgeFile = function () {
		// Use an arraybuffer
		$http.get("/api/main/GetEdgeFile", { responseType: 'arraybuffer' })
			.success(function (data, status, headers) {
				var octetStreamMime = 'application/octet-stream';
				var success = false;

				headers = headers();

				var filename = "Results_Edge.xlsx";

				var contentType = headers['content-type'] || octetStreamMime;

				try {
					var blob = new Blob([data], { type: contentType });
					if (navigator.msSaveBlob)
						navigator.msSaveBlob(blob, filename);
					else {
						var saveBlob = navigator.webkitSaveBlob || navigator.mozSaveBlob || navigator.saveBlob;
						if (saveBlob === undefined) throw "Not supported";
						saveBlob(blob, filename);
					}

					success = true;
				} catch (ex) {
				}

				if (!success) {
					var urlCreator = window.URL || window.webkitURL || window.mozURL || window.msURL;
					if (urlCreator) {
						var link = document.createElement('a');
						if ('download' in link) {
							try {
								var blob = new Blob([data], { type: contentType });
								var url = urlCreator.createObjectURL(blob);
								link.setAttribute('href', url);

								link.setAttribute("download", filename);

								var event = document.createEvent('MouseEvents');
								event.initMouseEvent('click', true, true, window, 1, 0, 0, 0, 0, false, false, false, false, 0, null);
								link.dispatchEvent(event);

								success = true;
							} catch (ex) {
							}
						}

						if (!success) {
							try {
								var blob = new Blob([data], { type: octetStreamMime });
								var url = urlCreator.createObjectURL(blob);
								window.location = url;

								success = true;
							} catch (ex) {
							}
						}
					}
				}

				if (!success) {
					window.open(httpPath, '_blank', '');
				}
			})
			.error(function (data, status) {
				$scope.errorDetails = "Request failed with status: " + status;
			});
	}
	vm.downloadCloudFile = function () {
		// Use an arraybuffer
		$http.get("api/main/GetCloudFile", { responseType: 'arraybuffer' })
			.success(function (data, status, headers) {
				var octetStreamMime = 'application/octet-stream';
				var success = false;

				headers = headers();

				var filename = "Results_Cloud.xlsx";

				var contentType = headers['content-type'] || octetStreamMime;

				try {
					var blob = new Blob([data], { type: contentType });
					if (navigator.msSaveBlob)
						navigator.msSaveBlob(blob, filename);
					else {
						var saveBlob = navigator.webkitSaveBlob || navigator.mozSaveBlob || navigator.saveBlob;
						if (saveBlob === undefined) throw "Not supported";
						saveBlob(blob, filename);
					}

					success = true;
				} catch (ex) {
				}

				if (!success) {
					var urlCreator = window.URL || window.webkitURL || window.mozURL || window.msURL;
					if (urlCreator) {
						var link = document.createElement('a');
						if ('download' in link) {
							try {
								var blob = new Blob([data], { type: contentType });
								var url = urlCreator.createObjectURL(blob);
								link.setAttribute('href', url);

								link.setAttribute("download", filename);

								var event = document.createEvent('MouseEvents');
								event.initMouseEvent('click', true, true, window, 1, 0, 0, 0, 0, false, false, false, false, 0, null);
								link.dispatchEvent(event);

								success = true;
							} catch (ex) {
							}
						}

						if (!success) {
							try {
								var blob = new Blob([data], { type: octetStreamMime });
								var url = urlCreator.createObjectURL(blob);
								window.location = url;

								success = true;
							} catch (ex) {
							}
						}
					}
				}

				if (!success) {
					window.open(httpPath, '_blank', '');
				}
			})
			.error(function (data, status) {
				$scope.errorDetails = "Request failed with status: " + status;
			});
	}
	// #endregion

	// #region checkbox toggle datacenter

	$scope.items = ['USA', 'Singapore'];
	$scope.selected = [];
	$scope.toggle = function (item, list) {
		var idx = list.indexOf(item);
		if (idx > -1) {
			list.splice(idx, 1);
		}
		else {
			list.push(item);
		}
	};
	$scope.exists = function (item, list) {
		return list.indexOf(item) > -1;
	};
	$scope.isIndeterminate = function () {
		return ($scope.selected.length !== 0 &&
			$scope.selected.length !== $scope.items.length);
	};
	$scope.isChecked = function () {
		return $scope.selected.length === $scope.items.length;
	};
	$scope.toggleAll = function () {
		if ($scope.selected.length === $scope.items.length) {
			$scope.selected = [];
		} else if ($scope.selected.length === 0 || $scope.selected.length > 0) {
			$scope.selected = $scope.items.slice(0);
		}
	};

	// #endregion
}]);

app.factory('fog', ['$http', '$q', '$window', function ($http, $q, $window) {
	var SimulateFogPost = function (FogPost, TuplePost, PolicyType, GatewayPolicyType, NodelevelPolicyTypes, communicationType, Service, DataCenter, Gateway, Cooperation, FogType, output) {
		var d = $q.defer();
		var model = { FogPost, TuplePost, PolicyType, GatewayPolicyType, NodelevelPolicyTypes, communicationType, Service, DataCenter, Gateway, Cooperation, FogType }
		// console.log(JSON.stringify(model));
		$http({
			method: "POST",
			url: "/api/main/fog",
			data: model,
			header: { 'Content-Type': "application/json", 'Accept': 'text/plain' }
		}).then(function (result) {
			//  console.log("result " + JSON.stringify(result));
			if (result.data == 'ok') {
				window.alert("Simulation has been done successfully.");
			} else { window.alert("There is some problem in executing code"); }
			output(result.data);
			d.resolve(result.data);
		}, function (error) {
			d.reject(error);
		});
		return d.promise;
	}
	return {
		SimulateFogPost: SimulateFogPost
	};
}]);
app.factory('edge', ['$http', '$q', '$window', function ($http, $q, $window) {
	var SimulateEdge = function (FogPost, TuplePost, PolicyType, communicationType, Service, DataCenter, Gateway, Cooperation, FogType, output) {
		var d = $q.defer();
		var model = { FogPost, TuplePost, PolicyType, communicationType, Service, DataCenter, Gateway, Cooperation, FogType }
		console.log(JSON.stringify(model));
		$http({
			method: "POST",
			url: "/api/main/edge",
			data: model,
			header: { 'Content-Type': "application/json", 'Accept': 'text/plain' }
		}).then(function (result) {
			if (result.data == 'ok') {
				window.alert("Simulation has been done successfully.");
			} else { window.alert("There is some problem in executing code"); }
			output(result.data);
			d.resolve(result.data);
		}, function (error) {
			d.reject(error);
		});
		return d.promise;
	}
	return {
		SimulateEdge: SimulateEdge
	};
}]);
app.factory('createDS', ['$http', '$q', function ($http, $q) {
	var GenerateFogData = function (FogPost, TuplePost, PolicyType, communicationType, Service, DataCenter, Gateway, Cooperation, FogType, output) {
		var d = $q.defer();
		var model = { FogPost, TuplePost, PolicyType, communicationType, Service, DataCenter, Gateway, Cooperation, FogType }
		$http({
			method: "POST",
			url: "/api/main/createDS",
			data: model,
			header: { 'Content-Type': "application/json", 'Accept': 'text/plain' },
		}).then(function (result) {

			console.log(result.data);
			if (result.data == "ok") {
				$('#floatBarsG').hide();
				$('#divError').text("Succefully created the Data Set. ");
			} else {
				$('#floatBarsG').hide();
				$('#divError').text("Some thing is went wrong please contact to technical person, Data Creation Failed.");
			}
			output(result.data);
			d.resolve(result.data);
		}, function (error) {
			$('#floatBarsG').hide();
			$('#divError').text("Some thing is went wrong please contact to technical person, Data Creation Failed.");
			d.reject(error);
		})
		return d.promise;
	};
	return {
		GenerateFogData: GenerateFogData
	};
}]);
app.factory('CreateEdgeDS', ['$http', '$q', function ($http, $q) {
	var GenerateEdgeData = function (FogPost, TuplePost, PolicyType, communicationType, Service, DataCenter, Gateway, Cooperation, FogType, output) {
		var d = $q.defer();
		var model = { FogPost, TuplePost, PolicyType, communicationType, Service, DataCenter, Gateway, Cooperation, FogType }
		$http({
			method: "POST",
			url: "/api/main/CreateEdgeDS",
			data: model,
			header: { 'Content-Type': "application/json", 'Accept': 'text/plain' },
		}).then(function (result) {

			console.log(result.data);
			if (result.data == "ok") {
				$('#EfloatBarsG').hide();
				$('#EdivError').text("Succefully created the Data Set. ");
			} else {
				$('#EfloatBarsG').hide();
				$('#EdivError').text("Some thing is went wrong please contact to technical person, Data Creation Failed.");
			}
			output(result.data);
			d.resolve(result.data);
		}, function (error) {
			$('#EfloatBarsG').hide();
			$('#EdivError').text("Some thing is went wrong please contact to technical person, Data Creation Failed.");
			d.reject(error);
		})
		return d.promise;
	};
	return {
		GenerateEdgeData: GenerateEdgeData
	};
}]);
app.factory('cloud', ['$http', '$q', function ($http, $q) {
	var SimulateCloudPost = function (TuplePost, PolicyType, Service, DataCenter, output) {
		var d = $q.defer();
		var model = { TuplePost, PolicyType, Service, DataCenter }
		$http({
			method: "POST",
			//url: "api/main/SimulateCloudPost",
			url: "api/main/cloud",
			data: model,
			header: { 'Content-Type': "application/json", 'Accept': 'text/plain' }
		}).then(function (result) {
			output(result.data);
			d.resolve(result.data);
		}, function (error) {
			d.reject(error);
		});
		return d.promise;
	}

	return {
		SimulateCloudPost: SimulateCloudPost
	};
}]);