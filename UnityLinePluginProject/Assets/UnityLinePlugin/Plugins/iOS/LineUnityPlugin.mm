#import <LineSDK/LineSDK.h>

extern "C" void UnitySendMessage(const char *, const char *, const char *);

@interface LineUnityPlugin : NSObject
{
    NSString *gameObjectName;
    NSString *channelId;
    id apiClient;
}
@end

@implementation LineUnityPlugin

- (id)initWithGameObjectName:(const char *)gameObjectName_
                   channelId:(const char *)channelId_
{
    self = [super init];
    gameObjectName = [NSString stringWithUTF8String:gameObjectName_];
    channelId = [NSString stringWithUTF8String:channelId_];
    apiClient = [[LineSDKAPI alloc] initWithConfiguration:[LineSDKConfiguration defaultConfig]];
    NSLog(@"init with :%@ ",gameObjectName);
    return self;
}

- (void)dispose
{
    [LineSDKLogin sharedInstance].delegate = nil;
    apiClient = nil;
    gameObjectName = nil;
    channelId = nil;
}

- (void)login
{
    NSLog(@"login with :%@ ",gameObjectName);
    [[LineSDKLogin sharedInstance] startLogin];
    // Set the LINE Login Delegate
    [LineSDKLogin sharedInstance].delegate = self;
}

- (void)loginWebView
{
    [[LineSDKLogin sharedInstance] startWebLogin];
    // Set the LINE Login Delegate
    [LineSDKLogin sharedInstance].delegate = self;
}

#pragma mark LineSDKLoginDelegate

- (void)didLogin:(LineSDKLogin *)login
      credential:(LineSDKCredential *)credential
         profile:(LineSDKProfile *)profile
           error:(NSError *)error
{
    NSLog(@"didLogin with :%@ %@",gameObjectName,error);
    if (error) {
        UnitySendMessage([gameObjectName UTF8String], "OnMessageApiError", [[self parseJsonWithError:error] UTF8String]);
    } else {
        UnitySendMessage([gameObjectName UTF8String], "OnMessageLoginSuccess", [[self parseJsonWithLogin:credential profile:profile] UTF8String]);
    }
}

- (void)logout
{
    [apiClient logoutWithCompletion:^(BOOL success, NSError * _Nullable error)
    {
        if (!success) {
            UnitySendMessage([gameObjectName UTF8String], "OnMessageApiError", [[self parseJsonWithError:error] UTF8String]);
        }
    }];
}

- (void)verifyToken
{
    [apiClient verifyTokenWithCompletion:^(LineSDKVerifyResult * _Nullable result, NSError * _Nullable error) {
        if (error) {
            UnitySendMessage([gameObjectName UTF8String], "OnMessageApiError", [[self parseJsonWithError:error] UTF8String]);
        } else {
            UnitySendMessage([gameObjectName UTF8String], "OnMessageVerifyResultReceived", [[self parseJsonWithVerifyResult:result] UTF8String]);
        }
    }];
}

- (void)getCurrentAccessToken
{
    LineSDKAccessToken *accessToken = [apiClient currentAccessToken];
    UnitySendMessage([gameObjectName UTF8String], "OnMessageAccessTokenReceived", [[self parseJsonWithAccessToken:accessToken] UTF8String]);
}

- (void)refreshToken
{
    [apiClient refreshTokenWithCompletion:^(LineSDKAccessToken *_Nullable accessToken, NSError *_Nullable error)
    {
        if (error) {
            UnitySendMessage([gameObjectName UTF8String], "OnMessageApiError", [[self parseJsonWithError:error] UTF8String]);
        } else {
            UnitySendMessage([gameObjectName UTF8String], "OnMessageAccessTokenReceived", [[self parseJsonWithAccessToken:accessToken] UTF8String]);
        }
         
    }];
}

- (void)getProfile
{
    [apiClient getProfileWithCompletion:^(LineSDKProfile * _Nullable profile, NSError * _Nullable error)
    {
        if(error) {
            UnitySendMessage([gameObjectName UTF8String], "OnMessageApiError", [[self parseJsonWithError:error] UTF8String]);
        } else {
            UnitySendMessage([gameObjectName UTF8String], "OnMessageProfileReceived", [[self parseJsonWithProfile:profile] UTF8String]);
        }
         
    }];
}

- (NSString *)parseJsonWithLogin:(LineSDKCredential *)credential
                             profile:(LineSDKProfile *)profile
{
    return [NSString stringWithFormat:@"{\"%@\":%@,\"%@\":%@}",
            @"credential",
            [self parseJsonWithCredential:credential],
            @"profile",
            [self parseJsonWithProfile:profile]];
}

- (NSString *)parseJsonWithAccessToken:(LineSDKAccessToken *)accessToken
{
    return [NSString stringWithFormat:@"{\"%@\":\"%@\",\"%@\":%ld,\"%@\":%ld}",
            @"accessToken",
            [accessToken accessToken],
            @"estimatedExpirationTimeMillis",
            (long)[[accessToken estimatedExpiredDate] timeIntervalSince1970] * 1000,
            @"expiresInMillis",
            (long)[accessToken expiresIn] * 1000
            ];
}

- (NSString *)parseJsonWithProfile:(LineSDKProfile *)profile
{
    return [NSString stringWithFormat:@"{\"%@\":\"%@\",\"%@\":\"%@\",\"%@\":\"%@\",\"%@\":\"%@\"}",
            @"userId",
            [profile userID],
            @"displayName",
            [profile displayName],
            @"pictureUrl",
            [[profile pictureURL] absoluteString],
            @"statusMessage",
            [profile statusMessage]
            ];
}

- (NSString *)parseJsonWithCredential:(LineSDKCredential *)credential
{
    return [NSString stringWithFormat:@"{\"%@\":%@,\"%@\":%@}",
            @"accessToken",
            [self parseJsonWithAccessToken:[credential accessToken]],
            @"permission",
            [self parseJsonWithStringSet:[credential permissions]]
            ];
}

- (NSString *)parseJsonWithVerifyResult:(LineSDKVerifyResult *)result
{
    return [NSString stringWithFormat:@"{\"%@\":%@,\"%@\":%@}",
            @"accessToken",
            @"dummy1",
            @"permission",
            [self parseJsonWithStringSet:[result permissions]]
            ];
}

- (NSString *)parseJsonWithError:(NSError*)error
{
    return [NSString stringWithFormat:@"{\"message\":\"%@\"}", error.description];
}

- (NSString *)parseJsonWithStringSet:(NSOrderedSet<NSString *> *)set
{
    NSString *setJson = @"";
    for (int i = 0; i < set.count; ++i) {
        if (i > 0)
            setJson = [NSString stringWithFormat:@"%@,",setJson];
        setJson = [NSString stringWithFormat:@"%@\"%@\"", setJson, set[i]];
    }
    setJson = [NSString stringWithFormat:@"[%@]", setJson];
    return setJson;
}

@end

extern "C" {
    void *_LineUnityPlugin_Init(const char *gameObjectName, const char *channelId);
    void _LineUnityPlugin_Login(void *instance);
    void _LineUnityPlugin_LoginWebView(void *instance);
    void _LineUnityPlugin_Logout(void *instance);
    void _LineUnityPlugin_VerifyToken(void *instance);
    void _LineUnityPlugin_GetCurrentAccessToken(void *instance);
    void _LineUnityPlugin_RefreshToken(void *instance);
    void _LineUnityPlugin_GetProfile(void *instance);
    void _LineUnityPlugin_Destroy(void *instance);
}

void *_LineUnityPlugin_Init(const char *gameObjectName, const char *channelId)
{
	id instance = [[LineUnityPlugin alloc] initWithGameObjectName:gameObjectName channelId: channelId];
	return (__bridge_retained void *)instance;
}

void _LineUnityPlugin_Login(void *instance)
{
    LineUnityPlugin *plugin = (__bridge LineUnityPlugin *)instance;
    [plugin login];
}

void _LineUnityPlugin_LoginWebView(void *instance)
{
    LineUnityPlugin *plugin = (__bridge LineUnityPlugin *)instance;
    [plugin loginWebView];
}

void _LineUnityPlugin_Logout(void *instance)
{
    LineUnityPlugin *plugin = (__bridge LineUnityPlugin *)instance;
    [plugin logout];
}

void _LineUnityPlugin_VerifyToken(void *instance)
{
    LineUnityPlugin *plugin = (__bridge LineUnityPlugin *)instance;
    [plugin verifyToken];
}

void _LineUnityPlugin_GetCurrentAccessToken(void *instance)
{
    LineUnityPlugin *plugin = (__bridge LineUnityPlugin *)instance;
    [plugin getCurrentAccessToken];
}

void _LineUnityPlugin_RefreshToken(void *instance)
{
    LineUnityPlugin *plugin = (__bridge LineUnityPlugin *)instance;
    [plugin refreshToken];
}

void _LineUnityPlugin_GetProfile(void *instance)
{
    LineUnityPlugin *plugin = (__bridge LineUnityPlugin *)instance;
    [plugin getProfile];
}
void _LineUnityPlugin_Destroy(void *instance)
{
    LineUnityPlugin *plugin = (__bridge_transfer LineUnityPlugin *)instance;
    [plugin dispose];
    plugin = nil;
}