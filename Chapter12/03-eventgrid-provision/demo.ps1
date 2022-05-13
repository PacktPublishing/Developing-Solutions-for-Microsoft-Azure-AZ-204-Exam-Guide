##########################################################################################
# This commands should be executed in PowerShell 
# You can install PowerShell locally or in  Cloud Shell
# Make sure you install Azure CLI  http://aka.ms/azcli
##########################################################################################

#values form previous run for Event Hub
$eventhub="";  # <<<<<--- please provide short name of eventhub from previous run
$groupname="EventHubDemo-RG"

# to avoid name collisions generate unique name for your account
$account="eventgri"+(Get-Random) 

#Create a resource group
New-AzResourceGroup -location eastus -name EventGridDemo-RG

#Enable the Event Grid resource provider
Register-AzResourceProvider -ProviderNamespace Microsoft.EventGrid

#Create a resource group to monitor
New-AzResourceGroup -location eastus -name EventGridMonitoring

#Pull azure subscription id
$subid=(az account show --query id -o tsv)

#Сonfigure event subscription endpoint
$endpoint="/subscriptions/$subid/resourceGroups/$groupname/providers/Microsoft.EventHub/namespaces/$eventhub/eventhubs/$eventhub"

#Create a subscription for the events from Resourece group
New-AzEventGridSubscription -EventSubscriptionName "group-monitor-sub"  -EndpointType "eventhub" -Endpoint $endpoint -ResourceGroup "EventGridMonitoring"

###################################################################
##  START EVENT HUB CONSUMER subscriber.exe FROM PREVIOUS DEMO   ##
###################################################################

######################################################## 
##  TO GENERATE ACTIVITY REPEAT THE FOLLOWING COMMAND  #
######################################################## 

# Update tag of monitoring RG. Required about 45 to appear in subscriber console
Set-AzResourceGroup -name EventGridMonitoring -Tag @{Code=(Get-Random)}

# do not delete the provision resources, it will be required for next step