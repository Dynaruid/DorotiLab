#nullable enable
#pragma warning disable CS0108, CS0162, CS0168, CS4014, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622
// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/autofill.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Services;

public abstract class AutofillHints
{
    public const string addressCity = "addressCity";
    public const string addressCityAndState = "addressCityAndState";
    public const string addressState = "addressState";
    public const string birthday = "birthday";
    public const string birthdayDay = "birthdayDay";
    public const string birthdayMonth = "birthdayMonth";
    public const string birthdayYear = "birthdayYear";
    public const string countryCode = "countryCode";
    public const string countryName = "countryName";
    public const string creditCardExpirationDate = "creditCardExpirationDate";
    public const string creditCardExpirationDay = "creditCardExpirationDay";
    public const string creditCardExpirationMonth = "creditCardExpirationMonth";
    public const string creditCardExpirationYear = "creditCardExpirationYear";
    public const string creditCardFamilyName = "creditCardFamilyName";
    public const string creditCardGivenName = "creditCardGivenName";
    public const string creditCardMiddleName = "creditCardMiddleName";
    public const string creditCardName = "creditCardName";
    public const string creditCardNumber = "creditCardNumber";
    public const string creditCardSecurityCode = "creditCardSecurityCode";
    public const string creditCardType = "creditCardType";
    public const string email = "email";
    public const string familyName = "familyName";
    public const string fullStreetAddress = "fullStreetAddress";
    public const string gender = "gender";
    public const string givenName = "givenName";
    public const string impp = "impp";
    public const string jobTitle = "jobTitle";
    public const string language = "language";
    public const string location = "location";
    public const string middleInitial = "middleInitial";
    public const string middleName = "middleName";
    public const string name = "name";
    public const string namePrefix = "namePrefix";
    public const string nameSuffix = "nameSuffix";
    public const string newPassword = "newPassword";
    public const string newUsername = "newUsername";
    public const string nickname = "nickname";
    public const string oneTimeCode = "oneTimeCode";
    public const string emailOTPCode = "emailOTPCode";
    public const string organizationName = "organizationName";
    public const string password = "password";
    public const string photo = "photo";
    public const string postalAddress = "postalAddress";
    public const string postalAddressExtended = "postalAddressExtended";
    public const string postalAddressExtendedPostalCode = "postalAddressExtendedPostalCode";
    public const string postalCode = "postalCode";
    public const string streetAddressLevel1 = "streetAddressLevel1";
    public const string streetAddressLevel2 = "streetAddressLevel2";
    public const string streetAddressLevel3 = "streetAddressLevel3";
    public const string streetAddressLevel4 = "streetAddressLevel4";
    public const string streetAddressLine1 = "streetAddressLine1";
    public const string streetAddressLine2 = "streetAddressLine2";
    public const string streetAddressLine3 = "streetAddressLine3";
    public const string sublocality = "sublocality";
    public const string telephoneNumber = "telephoneNumber";
    public const string telephoneNumberAreaCode = "telephoneNumberAreaCode";
    public const string telephoneNumberCountryCode = "telephoneNumberCountryCode";
    public const string telephoneNumberDevice = "telephoneNumberDevice";
    public const string telephoneNumberExtension = "telephoneNumberExtension";
    public const string telephoneNumberLocal = "telephoneNumberLocal";
    public const string telephoneNumberLocalPrefix = "telephoneNumberLocalPrefix";
    public const string telephoneNumberLocalSuffix = "telephoneNumberLocalSuffix";
    public const string telephoneNumberNational = "telephoneNumberNational";
    public const string transactionAmount = "transactionAmount";
    public const string transactionCurrency = "transactionCurrency";
    public const string url = "url";
    public const string username = "username";

}

public class AutofillConfiguration
{
    public static AutofillConfiguration disabled = new AutofillConfiguration(enabled: false, uniqueIdentifier: "", currentEditingValue: TextEditingValue.empty);
    public virtual bool enabled { get; private set; } = default!;
    public virtual string uniqueIdentifier { get; private set; } = default!;
    public virtual List<string> autofillHints { get; private set; } = default!;
    public virtual TextEditingValue currentEditingValue { get; private set; } = default!;
    public virtual string? hintText { get; private set; }

    public AutofillConfiguration(string uniqueIdentifier, List<string> autofillHints, TextEditingValue currentEditingValue, string? hintText = null)
        : this(enabled: true, uniqueIdentifier: uniqueIdentifier, autofillHints: autofillHints, currentEditingValue: currentEditingValue, hintText: hintText)
    {
    }

    public AutofillConfiguration(bool enabled, string uniqueIdentifier, List<string> autofillHints = default!, string? hintText = null, TextEditingValue currentEditingValue = default!)
    {
        this.enabled = enabled;
        this.uniqueIdentifier = uniqueIdentifier;
        this.autofillHints = autofillHints ?? new List<string>();
        this.hintText = hintText;
        this.currentEditingValue = currentEditingValue;
    }

    public virtual DartMap<string, object>? toJson()
    {
        return (enabled ? new DartMap<string, object> { ["uniqueIdentifier"] = uniqueIdentifier, ["hints"] = autofillHints, ["editingValue"] = currentEditingValue.toJSON(), ["hintText"] = hintText } : null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as AutofillConfiguration;
        if (__other is null) return false;
        if (ReferenceEquals(this, __other))
        {
            return true;
        }
        if ((!object.Equals(__other.GetType(), this.GetType())))
        {
            return false;
        }
        return ((((((__other is AutofillConfiguration) && (((AutofillConfiguration)__other).enabled == enabled)) && (((AutofillConfiguration)__other).uniqueIdentifier == uniqueIdentifier)) && global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.listEquals(((AutofillConfiguration)__other).autofillHints, autofillHints)) && (object.Equals(((AutofillConfiguration)__other).currentEditingValue, currentEditingValue))) && (((AutofillConfiguration)__other).hintText == hintText));
    }

    public override int GetHashCode()
    {
        return FoundationRuntimePorts.ObjectHash(enabled, uniqueIdentifier, FoundationRuntimePorts.ObjectHashAll(autofillHints), currentEditingValue, hintText);
    }
    public override string ToString()
    {
        var description = new List<string> { $"enabled: {enabled}", $"uniqueIdentifier: {uniqueIdentifier}", $"autofillHints: {autofillHints}", $"currentEditingValue: {currentEditingValue}" };
        return $"AutofillConfiguration({string.Join(", ", description)})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public interface AutofillClient
{
    public string autofillId { get; }
    public TextInputConfiguration textInputConfiguration { get; }
    public void autofill(TextEditingValue newEditingValue);
}

public interface AutofillScope
{
    public AutofillClient? getAutofillClient(string autofillId);
    public IEnumerable<AutofillClient> autofillClients { get; }
    public TextInputConnection attach(TextInputClient trigger, TextInputConfiguration configuration);
}

internal class _AutofillScopeTextInputConfiguration__autofill : TextInputConfiguration
{
    public virtual IEnumerable<TextInputConfiguration> allConfigurations { get; private set; } = default!;

    internal _AutofillScopeTextInputConfiguration__autofill(IEnumerable<TextInputConfiguration> allConfigurations, TextInputConfiguration currentClientConfiguration) : base(viewId: currentClientConfiguration.viewId, inputType: currentClientConfiguration.inputType, obscureText: currentClientConfiguration.obscureText, autocorrect: currentClientConfiguration.autocorrect, smartDashesType: currentClientConfiguration.smartDashesType, smartQuotesType: currentClientConfiguration.smartQuotesType, enableSuggestions: currentClientConfiguration.enableSuggestions, inputAction: currentClientConfiguration.inputAction, textCapitalization: currentClientConfiguration.textCapitalization, keyboardAppearance: currentClientConfiguration.keyboardAppearance, actionLabel: currentClientConfiguration.actionLabel, autofillConfiguration: currentClientConfiguration.autofillConfiguration)
    {
        this.allConfigurations = allConfigurations;
    }

    public override DartMap<string, object> toJson()
    {
        DartMap<string, object> result = base.toJson();
        result["fields"] = allConfigurations.map(((configuration) => configuration.toJson())).ToList();
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public interface AutofillScopeMixin : AutofillScope
{
    public TextInputConnection attach(TextInputClient trigger, TextInputConfiguration configuration)
    {
        DartRuntimePrimitives.Assert(() => !autofillClients.any(((client) => !client.textInputConfiguration.autofillConfiguration.enabled)));
        TextInputConfiguration inputConfiguration = new _AutofillScopeTextInputConfiguration__autofill(allConfigurations: autofillClients.map(((client) => client.textInputConfiguration)), currentClientConfiguration: configuration);
        return TextInput.attach(trigger, inputConfiguration);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
