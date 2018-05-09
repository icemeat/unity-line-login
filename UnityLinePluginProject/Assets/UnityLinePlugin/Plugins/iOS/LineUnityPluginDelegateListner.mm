#import "AppDelegateListener.h"
#import <LineSDK/LineSDK.h>

@interface AppDelegateListenerTest : NSObject <AppDelegateListener>
@end

@implementation AppDelegateListenerTest

static AppDelegateListenerTest *_instance = nil;

+ (void)load {
    if(!_instance) {
        _instance = [[AppDelegateListenerTest alloc] init];
    }
}

- (id)init {
    self = [super init];
    if(!self)
        return nil;
    
    _instance = self;
    
    // register to unity
    UnityRegisterAppDelegateListener(self);
    
    return self;
}

- (void)onOpenURL:(NSNotification*)notification {
    [[LineSDKLogin sharedInstance] handleOpenURL:[notification userInfo][@"url"]];
}

@end
